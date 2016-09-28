using System.Collections.Generic;
using System.Linq;
using CoursesAPI.Models;
using CoursesAPI.Services.DataAccess;
using CoursesAPI.Services.Exceptions;
using CoursesAPI.Services.Models.Entities;
using System;

namespace CoursesAPI.Services.Services
{
	public class CoursesServiceProvider
	{
		private readonly IUnitOfWork _uow;

		private readonly IRepository<CourseInstance> _courseInstances;
		private readonly IRepository<TeacherRegistration> _teacherRegistrations;
		private readonly IRepository<CourseTemplate> _courseTemplates; 
		private readonly IRepository<Person> _persons;

		public CoursesServiceProvider(IUnitOfWork uow)
		{
			_uow = uow;

			_courseInstances      = _uow.GetRepository<CourseInstance>();
			_courseTemplates      = _uow.GetRepository<CourseTemplate>();
			_teacherRegistrations = _uow.GetRepository<TeacherRegistration>();
			_persons              = _uow.GetRepository<Person>();
		}

		/// <summary>
		/// You should implement this function, such that all tests will pass.
		/// </summary>
		/// <param name="courseInstanceID">The ID of the course instance which the teacher will be registered to.</param>
		/// <param name="model">The data which indicates which person should be added as a teacher, and in what role.</param>
		/// <returns>Should return basic information about the person.</returns>
		public PersonDTO AddTeacherToCourse(int courseInstanceID, AddTeacherViewModel model)
		{
			//This function should add a person as a teacher to a course.
			//Each course may have more than one teacher.
			//Each course can have either 0 or 1 "main" teacher.
			//Each person can only be registered once as a teacher for a given course.	

			//This function should add a person as a teacher to a course.
			//Each course may have more than one teacher.
			//Each course can have either 0 or 1 "main" teacher.
			//Each person can only be registered once as a teacher for a given course.

			var teacher = (from x in _persons.All()
				where x.SSN == model.SSN
				select x).SingleOrDefault();

			if(teacher == null){
				throw new AppObjectNotFoundException();
			}

			var teacherInCourse = (from x in _teacherRegistrations.All()
			where x.CourseInstanceID == courseInstanceID
			select x).ToList();

			var course = (from x in _courseInstances.All()
				where x.ID == courseInstanceID
				select x).SingleOrDefault();
			if(course == null){
				throw new AppObjectNotFoundException();
			}

			for(int i = 0; i < teacherInCourse.Count(); i++){
				if(teacherInCourse[i].SSN == model.SSN){
					throw  new AppValidationException("PERSON_ALREADY_REGISTERED_TEACHER_IN_COURSE");
				}
				if(teacherInCourse[i].Type == TeacherType.MainTeacher){
					throw  new AppValidationException("COURSE_ALREADY_HAS_A_MAIN_TEACHER");
				}
			}

			var teacherReg = new TeacherRegistration {
				SSN = model.SSN,
				CourseInstanceID = courseInstanceID,
				Type = model.Type
			};
			_teacherRegistrations.Add(teacherReg);
			_uow.Save();

			var ret = (from x in _persons.All()
				where x.SSN == model.SSN
				select new PersonDTO{
					SSN = x.SSN,
					Name = x.Name
					}).SingleOrDefault();

			return ret;
		}

		/// <summary>
		/// You should write tests for this function. You will also need to
		/// modify it, such that it will correctly return the name of the main
		/// teacher of each course.
		/// </summary>
		/// <param name="semester"></param>
		/// <returns></returns>
		public List<CourseInstanceDTO> GetCourseInstancesBySemester(string semester = null, string acceptLang = null)
		{
			if (string.IsNullOrEmpty(semester))
			{
				semester = "20153";
			}

			// Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
			// Console.WriteLine(acceptLang);
			// Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");

			var courses = (from c in _courseInstances.All()
				join ct in _courseTemplates.All() on c.CourseID equals ct.CourseID
				where c.SemesterID == semester
				select new CourseInstanceDTO
				{
					Name               = ct.Name,
					TemplateID         = ct.CourseID,
					CourseInstanceID   = c.ID,
					MainTeacher        = ""
					
				}).ToList();

			if(acceptLang.StartsWith("en")){
				for(int i = 0; i < courses.Count(); i++){
					courses[i].Name = (from ct in _courseTemplates.All()
						where ct.CourseID == courses[i].TemplateID
						select ct.NameEN).SingleOrDefault();
				}
			}

			var teach = (from teacher in _teacherRegistrations.All()
				join person in _persons.All() on teacher.SSN equals person.SSN
				join course in _courseInstances.All() on teacher.CourseInstanceID equals course.ID
				where teacher.Type == TeacherType.MainTeacher && course.SemesterID == semester
				select person.Name).SingleOrDefault();

			
			if(teach != null){
				for(int i = 0; i < courses.Count();i++){
					courses[i].MainTeacher = teach;
				}
			}

			return courses;
		}
	}
}
