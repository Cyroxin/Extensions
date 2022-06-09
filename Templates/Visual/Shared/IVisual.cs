#define VISUALEXTENSIONV1

using System;

namespace Extensions.Templates.Visual.Shared
{
	public interface IVisual
	{
		public string Name { get; set; }
		public Uri Cover { get; set; }
	}
}
