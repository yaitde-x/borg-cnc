using System.IO;
using Yuni.Library;

namespace Borg.Machine
{
    public class RS274MetaBuilder {
        public RS274Meta Build(string id, string name, string description, string fileName) {
            var defaultProfileName = "default";

            return new RS274Meta() {
                Id = id, Name = name, Description = description, Profiles = new RS274MetaProfile[1] {
                    new RS274MetaProfile() {
                        Id = $"{id}-{defaultProfileName}",
                        ItemType = LibraryItemType.RS274,
                        ToolProfile = defaultProfileName,
                        SpindleProfile= defaultProfileName,
                        RelativePath = Path.Combine(id, defaultProfileName, fileName)
                    }
                }
            };
        }
    }
}