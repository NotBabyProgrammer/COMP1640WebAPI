﻿using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.BusinesLogic.DTO
{
    public class UsersDTOPut
    {
        public string? userName { get; set; }
        public string? password { get; set; }
        public int roleId { get; set; }
    }
}
