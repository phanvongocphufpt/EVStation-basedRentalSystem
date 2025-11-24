﻿using Repository.Entities;
using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class CitizenIdDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CitizenIdNumber { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
    }
    public class CreateCitizenIdDTO
    {
        public string Name { get; set; }
        public string CitizenIdNumber { get; set; }
        public DateOnly BirthDate { get; set; }
        public int UserId { get; set; }
    }
    public class UpdateCitizenIdStatusDTO
    {
        public int CitizenIdId { get; set; }
        public DocumentStatus Status { get; set; }
    }
    public class UpdateCitizenIdInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CitizenIdNumber { get; set; }
        public DateOnly BirthDate { get; set; }
    }
}