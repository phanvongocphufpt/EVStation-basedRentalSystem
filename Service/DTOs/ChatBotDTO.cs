using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
        public class ChatRequest
        {
            public string Message { get; set; }
            public bool ShortAnswer { get; set; } = false; // trả lời ngắn hay dài
        }

        public class ChatResponse
        {
            public string Reply { get; set; }
        }
    }

