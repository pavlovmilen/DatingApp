using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository: IMessageRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            _dataContext.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _dataContext.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _dataContext.Messages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _dataContext.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(m => m.Recipient.UserName == messageParams.Username && 
                                            m.RecipientDeleted == false),
                "Outbox" => query.Where(m => m.Sender.UserName == messageParams.Username &&
                                             m.SenderDeleted == false),
                _ => query.Where(m => m.Recipient.UserName == messageParams.Username && 
                                      m.DateRead == null &&
                                      m.RecipientDeleted == false)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _dataContext.Messages
                .Include(m => m.Sender).ThenInclude(p => p.Photos)
                .Include(m => m.Recipient).ThenInclude(p => p.Photos)
                .Where(m => (m.Recipient.UserName == currentUsername && m.RecipientDeleted == false && m.Sender.UserName == recipientUsername) ||
                            m.Recipient.UserName == recipientUsername && m.SenderDeleted == false && m.Sender.UserName == currentUsername)
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

            var unreadMessages = messages
                .Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername)
                .ToList();

            if (unreadMessages.Any())
            {
                foreach (var unreadMessage in unreadMessages)
                {
                    unreadMessage.DateRead = DateTime.Now;
                }

                await _dataContext.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
