﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clients.DTOs
{
    public class UserInfoDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public AddressDto? Address { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public CompanyDto? Company { get; set; }
    }
}
