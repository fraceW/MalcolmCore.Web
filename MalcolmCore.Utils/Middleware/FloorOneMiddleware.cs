using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MalcolmCore.Utils.Middleware
{
    public class FloorOneMiddleware
    {
        private readonly RequestDelegate _next;
        public FloorOneMiddleware(RequestDelegate next) 
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) 
        {
            int n = 2;
            List<List<int>> startEnd = new List<List<int>>() { new List<int>() { 1,2},new List<int>() { 2,3} };
            int count = 0;
            int[] start = new int[n];
            int[] end = new int[n];
            for (int i = 0; i < startEnd.Count; i++)
            {
                start[i] = startEnd[i][0];
                end[i] = startEnd[i][1];
            }
            Array.Sort(start);
            Array.Sort(end);

            int index = 0;
            for (int i = 0; i < start.Length; i++)
            {
                if (start[i] < end[index])
                {
                    count++;
                }
                else
                {
                    index++;
                }
            }



            Console.WriteLine("FloorOneMiddleware In");
            await _next(context);
            Console.WriteLine("FloorOneMiddleware Out");
        }
    }
}
