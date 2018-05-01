using AutoMapper;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Module;
using BBM.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Logic
{
    public class ChannelBusiness : IChannelBusiness
    {
        private IUnitOfWork unitOfWork;

        public ChannelBusiness(
            IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public List<ChannelModel> GetChannel()
        {
            return Mapper.Map<List<ChannelModel>>(unitOfWork.ChannelRepository.GetAll().ToList());
        }

        public async Task<bool> CreateChannel(ChannelModel model, int UserId)
        {
            var objchannel = Mapper.Map<soft_Channel>(model);

            objchannel.DateCreate = DateTime.Now;

            objchannel.EmployeeCreate = UserId;

            objchannel.DateUpdate = null;

            unitOfWork.ChannelRepository.Add(objchannel);

            await unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> UpdateChannel(ChannelModel model, int UserId)
        {
            if (model.Id <= 0)
                return false;

            var channel = unitOfWork.ChannelRepository.GetById(model.Id);
            if (channel == null)
                return false;

            channel.DateUpdate = DateTime.Now;
            channel.EmployeeUpdate = UserId;
            channel.Code = model.Code;
            channel.Channel = model.Channel;
            channel.Note = model.Note;
            channel.Type = model.Type;


            unitOfWork.ChannelRepository.Update(channel,
                o => o.Code,
                o => o.Channel,
                o => o.Note,
                o => o.Type,
                o => o.DateUpdate,
                o => o.EmployeeUpdate);

            await unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> RemoveChannel(int channelId)
        {
            var channel = unitOfWork.ChannelRepository.GetById(channelId);

            if (channel == null)
                return false;

            unitOfWork.ChannelRepository.Delete(channel);

            await unitOfWork.SaveChanges();

            return true;
        }

    }
}
