﻿namespace DDM.Services.Permission
{
    public interface IPermissionService
    {
        bool HasPermission(string key);
    }
}