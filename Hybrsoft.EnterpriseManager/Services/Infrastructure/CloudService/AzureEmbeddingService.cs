using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Services;
using Microsoft.Data.SqlTypes;
using OpenAI;
using OpenAI.Embeddings;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class AzureEmbeddingService : IEmbeddingService
	{
		private readonly EmbeddingClient _client;

		private bool _isConfigured;
		public bool IsConfigured => _isConfigured;
		public int EmbeddingDimension => AppConfig.Azure_OpenAI_Embedding_Dimension;

		public AzureEmbeddingService(string modelName, string apiKey, Uri endpoint)
		{

			if (string.IsNullOrWhiteSpace(modelName) || string.IsNullOrWhiteSpace(apiKey) || endpoint == null)
			{
				DisableService();
				return;
			}

			try
			{
				var credential = new ApiKeyCredential(apiKey);

				_client = new EmbeddingClient(
					modelName,
					credential: credential,
					options: new OpenAIClientOptions()
					{
						Endpoint = endpoint
					}
				);

				_isConfigured = true;
			}
			catch
			{
				DisableService();
			}
		}

		public async Task<SqlVector<float>> GenerateEmbeddingAsync(string input)
		{
			if (!IsConfigured)
				return CreateVectorNull();

			if (string.IsNullOrWhiteSpace(input))
				throw new ArgumentException("Text cannot be empty.", nameof(input));

			try
			{
				OpenAIEmbedding embedding = await _client.GenerateEmbeddingAsync(input);
				return new SqlVector<float>(embedding.ToFloats().ToArray());
			}
			catch (Exception ex)
			{
				await HandleFailureAsync("Generate Embedding", ex);
				return CreateVectorNull();
			}
		}

		public async Task<IReadOnlyList<SqlVector<float>>> GenerateEmbeddingsAsync(IEnumerable<string> inputs)
		{
			ArgumentNullException.ThrowIfNull(inputs);

			if (!IsConfigured)
			{
				return [.. Enumerable.Repeat(CreateVectorNull(), inputs?.Count() ?? 0)];
			}

			try
			{
				OpenAIEmbeddingCollection response = await _client.GenerateEmbeddingsAsync(inputs);

				return [.. response.Select(embedding => new SqlVector<float>(embedding.ToFloats().ToArray()))];
			}
			catch (Exception ex)
			{
				await HandleFailureAsync("Generate Embeddings", ex);
				return [.. Enumerable.Repeat(CreateVectorNull(), inputs?.Count() ?? 0)];
			}
		}

		private static SqlVector<float> CreateVectorNull()
		{
			return SqlVector<float>.CreateNull(AppConfig.Azure_OpenAI_Embedding_Dimension);
		}

		private async Task HandleFailureAsync(string operation, Exception ex)
		{
			DisableService();

			ISettingsService settingsService = ServiceLocator.Current.GetService<ISettingsService>();
			settingsService.UseSemanticSearch = false;
			settingsService.IsSemanticSearchEnabled = false;

			ILogService logService = ServiceLocator.Current.GetService<ILogService>();
			await logService.WriteAsync(LogType.Error, "Azure Embedding Service", operation, ex);
		}

		private void DisableService()
		{
			_isConfigured = false;
		}
	}
}
