using BBM.Business.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public interface IChannelBusiness
    {
        List<ChannelModel> GetChannel();
        Task<bool> CreateChannel(ChannelModel model, int UserId);
        Task<bool> UpdateChannel(ChannelModel model, int UserId);
        Task<bool> RemoveChannel(int channelId);
    }
}
