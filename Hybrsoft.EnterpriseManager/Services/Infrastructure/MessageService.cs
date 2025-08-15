using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class MessageService : IMessageService
	{
		private readonly Lock _sync = new();

		private readonly List<Subscriber> _subscribers = [];

		public void Subscribe<TSender>(object target, Action<TSender, string, object> action) where TSender : class
		{
			Subscribe<TSender, Object>(target, action);
		}
		public void Subscribe<TSender, TArgs>(object target, Action<TSender, string, TArgs> action) where TSender : class
		{
			ArgumentNullException.ThrowIfNull(target);
			ArgumentNullException.ThrowIfNull(action);

			lock (_sync)
			{
				var subscriber = _subscribers.Where(r => r.Target == target).FirstOrDefault();
				if (subscriber == null)
				{
					subscriber = new Subscriber(target);
					_subscribers.Add(subscriber);
				}
				subscriber.AddSubscription<TSender, TArgs>(action);
			}
		}

		public void Unsubscribe<TSender>(object target) where TSender : class
		{
			ArgumentNullException.ThrowIfNull(target);

			lock (_sync)
			{
				var subscriber = _subscribers.Where(r => r.Target == target).FirstOrDefault();
				if (subscriber != null)
				{
					subscriber.RemoveSubscription<TSender>();
					if (subscriber.IsEmpty)
					{
						_subscribers.Remove(subscriber);
					}
				}
			}
		}
		public void Unsubscribe<TSender, TArgs>(object target) where TSender : class
		{
			ArgumentNullException.ThrowIfNull(target);

			lock (_sync)
			{
				var subscriber = _subscribers.Where(r => r.Target == target).FirstOrDefault();
				if (subscriber != null)
				{
					subscriber.RemoveSubscription<TSender, TArgs>();
					if (subscriber.IsEmpty)
					{
						_subscribers.Remove(subscriber);
					}
				}
			}
		}
		public void Unsubscribe(object target)
		{
			ArgumentNullException.ThrowIfNull(target);

			lock (_sync)
			{
				var subscriber = _subscribers.Where(r => r.Target == target).FirstOrDefault();
				if (subscriber != null)
				{
					_subscribers.Remove(subscriber);
				}
			}
		}

		public void Send<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class
		{
			ArgumentNullException.ThrowIfNull(sender);

			foreach (var subscriber in GetSubscribersSnapshot())
			{
				// Avoid sending message to self
				if (subscriber.Target != sender)
				{
					subscriber.TryInvoke(sender, message, args);
				}
			}
		}

		public void SendYourself<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class
		{
			ArgumentNullException.ThrowIfNull(sender);

			var subscriber = _subscribers
				.Where(r => r.Target == sender)
				.FirstOrDefault();

			if (subscriber == null)
				return; // No subscriber for the sender

			subscriber.TryInvoke(sender, message, args);
		}

		private Subscriber[] GetSubscribersSnapshot()
		{
			lock (_sync)
			{
				return [.. _subscribers];
			}
		}

		class Subscriber(object target)
		{
			private readonly WeakReference _reference = new(target);

			private readonly Dictionary<Type, Subscriptions> _subscriptions = [];

			public object Target => _reference.Target;

			public bool IsEmpty => _subscriptions.Count == 0;

			public void AddSubscription<TSender, TArgs>(Action<TSender, string, TArgs> action)
			{
				if (!_subscriptions.TryGetValue(typeof(TSender), out Subscriptions subscriptions))
				{
					subscriptions = new Subscriptions();
					_subscriptions.Add(typeof(TSender), subscriptions);
				}
				subscriptions.AddSubscription(action);
			}

			public void RemoveSubscription<TSender>()
			{
				_subscriptions.Remove(typeof(TSender));
			}
			public void RemoveSubscription<TSender, TArgs>()
			{
				if (_subscriptions.TryGetValue(typeof(TSender), out Subscriptions subscriptions))
				{
					subscriptions.RemoveSubscription<TArgs>();
					if (subscriptions.IsEmpty)
					{
						_subscriptions.Remove(typeof(TSender));
					}
				}
			}

			public void TryInvoke<TArgs>(object sender, string message, TArgs args)
			{
				var target = _reference.Target;
				if (_reference.IsAlive)
				{
					var senderType = sender.GetType();
					foreach (var keyValue in _subscriptions.Where(r => r.Key.IsAssignableFrom(senderType)))
					{
						var subscriptions = keyValue.Value;
						subscriptions.TryInvoke(sender, message, args);
					}
				}
			}
		}

		class Subscriptions
		{
			private readonly Dictionary<Type, Delegate> _subscriptions = null;

			public Subscriptions()
			{
				_subscriptions = [];
			}

			public bool IsEmpty => _subscriptions.Count == 0;

			public void AddSubscription<TSender, TArgs>(Action<TSender, string, TArgs> action)
			{
				_subscriptions.Add(typeof(TArgs), action);
			}

			public void RemoveSubscription<TArgs>()
			{
				_subscriptions.Remove(typeof(TArgs));
			}

			public void TryInvoke<TArgs>(object sender, string message, TArgs args)
			{
				var argsType = typeof(TArgs);
				foreach (var keyValue in _subscriptions.Where(r => r.Key.IsAssignableFrom(argsType)))
				{
					var action = keyValue.Value;
					action?.DynamicInvoke(sender, message, args);
				}
			}
		}
	}
}
