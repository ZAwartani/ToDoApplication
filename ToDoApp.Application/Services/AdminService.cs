using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Interfaces.Specification;
using ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Filter;
using ToDoApp.Application.Interfaces.Specification.SpecificationQuery.Searching.CategoryQ;
using ToDoApp.Domain.Entity;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IGenericRepository<Users, int> _genericRepositoryUser;
        private readonly IGenericRepository<ToDoItem, int> _genericRepositoryTodoItem;
        private readonly IGenericRepository<Category, int> _genericRepositoryCategory;
        public AdminService(IGenericRepository<Users, int> genericRepositoryUser, IGenericRepository<ToDoItem, int> genericRepositoryTodoItem, IGenericRepository<Category, int> genericRepositoryCategory)
        {
            _genericRepositoryUser = genericRepositoryUser;
            _genericRepositoryTodoItem = genericRepositoryTodoItem;
            _genericRepositoryCategory = genericRepositoryCategory;
        }
        public async Task<bool> ChangePasswordUserByAdmin(
           AdminRestPasswordUserDto adminRestPasswordUserDto)
        {
            var spec = new GetListByEmail(adminRestPasswordUserDto.Email);

            var user = (await _genericRepositoryUser.ListAsync(spec))
                .SingleOrDefault();

            if (user == null)
                return false;


            var passwordHash = BCrypt.Net.BCrypt
                .HashPassword(adminRestPasswordUserDto.Password);


            user.UpdatePasswordByAdmin(passwordHash);


            await _genericRepositoryUser.UpdateAsync(user);

            return true;
        }

        public async Task<IReadOnlyList<UserListDTO>> ListOfUserForAdmin()
        {
            var users = await _genericRepositoryUser.GetAllAsync();


            return users.Select(x => new UserListDTO(
                    x.Id,
                    x.Email
                ))
                .ToList();
        }


        public async Task<bool> AdminCanCreateAdminUser(
    RegisterNewUserDto newUser)
        {
            var emailSpec = new GetListByEmail(newUser.Email);

            var existingUser =
                (await _genericRepositoryUser.ListAsync(emailSpec))
                .SingleOrDefault();

            if (existingUser != null)
                return false;

            var passwordHash =
                BCrypt.Net.BCrypt.HashPassword(newUser.Password);

            var user = new Users(
                newUser.UserName,
                newUser.Email,
                passwordHash,
                enumUserRole.UserAdmin
            );

            await _genericRepositoryUser.AddAsync(user);

            return true;
        }

        public async Task<bool> RemoveUserAsync(string email)
        {
            var spec = new GetListByEmail(email);

            var user =
                (await _genericRepositoryUser.ListAsync(spec))
                .SingleOrDefault();

            if (user == null)
                return false;

            await _genericRepositoryUser.DeleteAsync(user.Id);

            return true;
        }

        public async Task<bool> UpdateTodoForUser(UpdateTodoDto updateTodoDto , int Id)
        {
            var todo = await _genericRepositoryTodoItem.GetByIdAsync(Id);

            if (todo == null)

                return false;
            var spec = new SearchingTitleOnCategory(updateTodoDto.TitleCategory);
            var resultspec = await _genericRepositoryCategory.ListAsync(spec);
            if (!resultspec.Any()) return false;
            var categoricalId = resultspec.First().Id;
            todo.Update(
                updateTodoDto.Title,
                updateTodoDto.Description,
                updateTodoDto.Priority,
                updateTodoDto.ExpireDate,
                categoricalId,
                updateTodoDto.Recurring
            );


            await _genericRepositoryTodoItem.UpdateAsync(todo);
            return true;
        }
    }
}
