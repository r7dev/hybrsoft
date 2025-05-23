﻿using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class ResourceService : IResourceService
	{
		private const string LanguageSettingsKey = "Language";

		private readonly ResourceManager _resourceManager;
		private readonly ResourceContext _resourceContext;

		private LanguageDto _currentLanguageItem = new() { DisplayName = "English", Tag = "en-US" };
		public List<LanguageDto> LanguageOptions { get; } = [];

		public ResourceService(ISettingsService settingsService)
		{
			_resourceManager = new ResourceManager();
			_resourceContext = _resourceManager.CreateResourceContext();
			SettingsService = settingsService;
		}

		public ISettingsService SettingsService { get; }

		public async Task InitializeAsync()
		{
			RegisterLanguageFromResource();

			string savedLanguageTag = await GetLanguageTagFromSettingsAsync();

			if (savedLanguageTag is not null && GetLanguageItem(savedLanguageTag) is LanguageDto savedLanguage)
			{
				await SetLanguageAsync(savedLanguage);
			}
			else
			{
				string systemLanguageTag = CultureInfo.CurrentCulture.Name;
				if (GetLanguageItem(systemLanguageTag) is LanguageDto systemLanguage)
				{
					await SetLanguageAsync(systemLanguage);
				}
				else
				{
					await SetLanguageAsync(_currentLanguageItem);
				}
			}
		}

		public LanguageDto GetCurrentLanguageItem() => _currentLanguageItem;

		public string GetString(string resourceFile, string key)
		{
			var resourceMap = _resourceManager.MainResourceMap.GetSubtree(resourceFile);
			try
			{
				var resource = resourceMap.GetValue(key, _resourceContext);
				return resource?.ValueAsString ?? $"[{resourceFile}/{key} not found]";
			}
			catch (Exception ex)
			{
				throw new ArgumentException($"Error fetching resource {resourceFile}/{key}", ex);
			}
		}

		public async Task SetLanguageAsync(LanguageDto language)
		{
			if (LanguageOptions.Contains(language) is true)
			{
				_currentLanguageItem = language;

				ApplicationLanguages.PrimaryLanguageOverride = language.Tag;
				_resourceContext.QualifierValues["Language"] = language.Tag;

				await SettingsService.SaveSettingAsync(LanguageSettingsKey, language.Tag);
			}
		}

		public ResourceContext GetContext()
		{
			return _resourceContext;
		}

		private LanguageDto GetLanguageItem(string tag)
		{
			return LanguageOptions.FirstOrDefault(item => item.Tag == tag);
		}

		private async Task<string> GetLanguageTagFromSettingsAsync()
		{
			return await SettingsService.ReadSettingAsync<string>(LanguageSettingsKey);
		}

		private void RegisterLanguageFromResource()
		{
			ResourceMap resourceMap = _resourceManager.MainResourceMap.GetSubtree("LanguageOptions");

			for (uint i = 0; i < resourceMap.ResourceCount; i++)
			{
				var resource = resourceMap.GetValueByIndex(i);
				LanguageOptions.Add(new LanguageDto() { DisplayName = resource.Value.ValueAsString, Tag = resource.Key });
			}
		}
	}
}
