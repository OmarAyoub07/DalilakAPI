using System.Collections.Generic;
using System.Linq;

namespace DalilakAPI.Classes
{
    public class StoreImages
    {
        private static List<string[]> imagesDuringProcess = new List<string[]>();

        public void SetPacket(string id, string packet)
        {
            if (imagesDuringProcess.Any(ids => ids[0] == id))
            {
                imagesDuringProcess.Single(ids => ids[0] == id)[1] += packet;
            }
            else
            {
                imagesDuringProcess.Add(new string[] {id, packet});
            }
        } 

        public string GetImage(string id)
        {
            return imagesDuringProcess.Single(ids => ids[0] == id)[1];
        }

        public void RemoveItem(string id)
        {
            var item = imagesDuringProcess.Single(ids => ids[0] == id);
            imagesDuringProcess.Remove(item);
        }
    }
}
