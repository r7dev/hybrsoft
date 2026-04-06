using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.UI.Windows.Models
{
	public partial class LostAndFoundModel : ObservableObject
	{
		public static LostAndFoundModel CreateEmpty() => new() { LostAndFoundID = -1, IsEmpty = true };
		public long LostAndFoundID { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public short LostAndFoundStatusID { get; set; }
		public long? StudentBelongingID { get; set; }
		public DateTimeOffset? DonationDate { get; set; }

		public bool IsNew => LostAndFoundID <= 0;

		public byte[] Picture { get; set; }
		public object PictureSource { get; set; }

		public byte[] Thumbnail { get; set; }
		public object ThumbnailSource { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public virtual LostAndFoundStatusModel LostAndFoundStatus { get; set; }

		public override void Merge(ObservableObject source)
		{
			if (source is LostAndFoundModel model)
			{
				Merge(model);
			}
		}

		public void Merge(LostAndFoundModel source)
		{
			if (source != null)
			{
				LostAndFoundID = source.LostAndFoundID;
				DisplayName = source.DisplayName;
				Description = source.Description;
				LostAndFoundStatusID = source.LostAndFoundStatusID;
				StudentBelongingID = source.StudentBelongingID;
				DonationDate = source.DonationDate;
				Picture = source.Picture;
				PictureSource = source.PictureSource;
				Thumbnail = source.Thumbnail;
				ThumbnailSource = source.ThumbnailSource;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
