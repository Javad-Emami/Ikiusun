﻿using Application.Interfaces.Repository;
using Domain.Entites;

namespace Application.Interfaces;

public interface IUserService: IBaseService<User,Guid>
{
}
