using Microsoft.AspNetCore.Mvc;
using System;

using CoursesAPI.Models;
using CoursesAPI.Services.DataAccess;
using CoursesAPI.Services.Services;

namespace CoursesAPI.Controllers
{
	[Route("api/courses")]
	public class CoursesController : Controller
	{
		private readonly CoursesServiceProvider _service;

		public CoursesController(IUnitOfWork uow)
		{
			_service = new CoursesServiceProvider(uow);
		}

		[HttpGet]
		public IActionResult GetCoursesBySemester(string semester = null, string pageNr = null)
		{
			// TODO: figure out the requested language (if any!)
			// and pass it to the service provider!
			//Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
			//Console.WriteLine(Request.Headers["Accept-Language"]);
			//Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");

			var acceptLang = Request.Headers["Accept-Language"];

			return Ok(_service.GetCourseInstancesBySemester(semester, acceptLang, pageNr));
		}

		/// <summary>
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("{id}/teachers")]
		public IActionResult AddTeacher(int id, AddTeacherViewModel model)
		{
			var result = _service.AddTeacherToCourse(id, model);
			return Created("TODO", result);
		}
	}
}
