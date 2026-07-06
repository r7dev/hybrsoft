using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.UI.Windows.Services;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Embeddings;
using System;
using System.ClientModel;

namespace Hybrsoft.EnterpriseManager.Services
{
	public static class EmbeddingServiceFactory
	{
		public static IEmbeddingService CreateAzure(string modelName, string apiKey, Uri endpoint)
		{
			int dimensions = AppConfig.Azure_OpenAI_Embedding_Dimension;

			if (string.IsNullOrWhiteSpace(modelName) || string.IsNullOrWhiteSpace(apiKey) || endpoint == null)
				return Disabled(dimensions, "Azure Embedding Service");

			try
			{
				var credential = new ApiKeyCredential(apiKey);

				var client = new EmbeddingClient(
					modelName,
					credential: credential,
					options: new OpenAIClientOptions { Endpoint = endpoint });

				IEmbeddingGenerator<string, Embedding<float>> generator =
					client.AsIEmbeddingGenerator(defaultModelDimensions: dimensions);

				return new EmbeddingServiceAdapter(generator, dimensions, "Azure Embedding Service");
			}
			catch
			{
				return Disabled(dimensions, "Azure Embedding Service");
			}
		}

		private static EmbeddingServiceAdapter Disabled(int dimensions, string logSourceName)
		{
			return new EmbeddingServiceAdapter(generator: null, dimensions, logSourceName);
		}
	}
}
