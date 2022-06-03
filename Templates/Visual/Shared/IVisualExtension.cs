#define VISUALEXTENSIONV1

using Extensions.Templates.Shared;
using System.Threading.Tasks;

namespace Extensions.Templates.Visual.Shared
{
	public interface IVisualExtension : IExtension
	{
		public Task<IVisual[]> GetVisuals();
	}
	public interface IVisualExtensionSearchable : IVisualExtension
	{
		public Task<IVisual[]> SearchVisuals(string name);
	}

	public interface IVisualExtensionPaginatable : IVisualExtension
	{
		public Task<(IVisual[] visuals, bool canLoadMore)> GetVisuals(int page = 0);
	}

}
