using AutoMapper;
using CustomEd.Notification.Service.Models;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Notification.Service.Consumers.ClassRoomConsumers;

public class ClassRoomDeletedEventConsumer
{
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ClassRoom> _classRoomRepository;
        

        public ClassRoomDeletedEventConsumer(IMapper mapper, IGenericRepository<ClassRoom> classRoomRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;        
        }


}