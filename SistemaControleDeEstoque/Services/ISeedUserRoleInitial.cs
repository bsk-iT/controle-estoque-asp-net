﻿namespace SistemaControleDeEstoque.Services;

public interface ISeedUserRoleInitial
{
    Task SeedRolesAsync();
    Task SeedUsersAsync();
}
