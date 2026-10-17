using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagementSystem
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }

        public Instructor(int instructorId, string name, string specialization)
        {
            InstructorId = instructorId;
            Name = name;
            Specialization = specialization;
        }

        public string PrintDetails()
        {
            return $"Instructor ID: {InstructorId} | Name: {Name} | Specialization: {Specialization}";
        }
    }

    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public Instructor Instructor { get; set; }

        public Course(int courseId, string title, Instructor instructor)
        {
            CourseId = courseId;
            Title = title;
            Instructor = instructor;
        }

        public string PrintDetails()
        {
            string instructorName = Instructor != null ? Instructor.Name : "No Instructor Assigned";
            return $"Course ID: {CourseId} | Title: {Title} | Instructor: {instructorName}";
        }
    }

    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public List<Course> Courses { get; set; }

        public Student(int studentId, string name, int age)
        {
            StudentId = studentId;
            Name = name;
            Age = age;
            Courses = new List<Course>();
        }

        public bool Enroll(Course course)
        {
            if (course == null) return false;

            if (Courses.Any(c => c.CourseId == course.CourseId))
            {
                return false;
            }

            Courses.Add(course);
            return true;
        }

        public string PrintDetails()
        {
            string courseList = Courses.Count > 0
                ? string.Join(", ", Courses.Select(c => c.Title))
                : "None";

            return $"Student ID: {StudentId} | Name: {Name} | Age: {Age} | Enrolled Courses: [{courseList}]";
        }
    }

    public class StudentManager
    {
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Instructor> Instructors { get; set; } = new List<Instructor>();

        public bool AddStudent(Student student)
        {
            if (student == null || Students.Any(s => s.StudentId == student.StudentId))
                return false;

            Students.Add(student);
            return true;
        }

        public bool AddCourse(Course course)
        {
            if (course == null || Courses.Any(c => c.CourseId == course.CourseId))
                return false;

            Courses.Add(course);
            return true;
        }

        public bool AddInstructor(Instructor instructor)
        {
            if (instructor == null || Instructors.Any(i => i.InstructorId == instructor.InstructorId))
                return false;

            Instructors.Add(instructor);
            return true;
        }

        public Student FindStudent(int studentId)
        {
            return Students.FirstOrDefault(s => s.StudentId == studentId);
        }

        public Course FindCourse(int courseId)
        {
            return Courses.FirstOrDefault(c => c.CourseId == courseId);
        }

        public Instructor FindInstructor(int instructorId)
        {
            return Instructors.FirstOrDefault(i => i.InstructorId == instructorId);
        }

        public bool EnrollStudentInCourse(int studentId, int courseId)
        {
            var student = FindStudent(studentId);
            var course = FindCourse(courseId);

            if (student != null && course != null)
            {
                return student.Enroll(course);
            }

            return false;
        }

        public bool IsStudentEnrolledInCourse(int studentId, int courseId)
        {
            var student = FindStudent(studentId);
            if (student == null) return false;

            return student.Courses.Any(c => c.CourseId == courseId);
        }

        public string GetInstructorNameByCourseName(string courseName)
        {
            var course = Courses.FirstOrDefault(c => c.Title.Equals(courseName, StringComparison.OrdinalIgnoreCase));
            if (course != null && course.Instructor != null)
            {
                return course.Instructor.Name;
            }
            return "Course or Instructor Not Found";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            StudentManager manager = new StudentManager();
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n=================== Student Management System ===================");
                Console.WriteLine("1. Add Student");
                Console.WriteLine("2. Add Instructor");
                Console.WriteLine("3. Add Course");
                Console.WriteLine("4. Enroll Student in Course");
                Console.WriteLine("5. Show All Students");
                Console.WriteLine("6. Show All Courses");
                Console.WriteLine("7. Show All Instructors");
                Console.WriteLine("8. Find Student (by ID or Name)");
                Console.WriteLine("9. Find Course (by ID or Name)");
                Console.WriteLine("10. Exit");
                Console.WriteLine("--- BONUS ---");
                Console.WriteLine("11. Check if Student is Enrolled in Specific Course");
                Console.WriteLine("12. Get Instructor Name by Course Name");
                Console.Write("Enter your choice (1-12): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Student ID: ");
                        int sId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Student Name: ");
                        string sName = Console.ReadLine();
                        Console.Write("Enter Student Age: ");
                        int sAge = int.Parse(Console.ReadLine());

                        if (manager.AddStudent(new Student(sId, sName, sAge)))
                            Console.WriteLine("--> Student added successfully!");
                        else
                            Console.WriteLine("--> Failed! Student ID already exists.");
                        break;

                    case "2":
                        Console.Write("Enter Instructor ID: ");
                        int iId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Instructor Name: ");
                        string iName = Console.ReadLine();
                        Console.Write("Enter Specialization: ");
                        string spec = Console.ReadLine();

                        if (manager.AddInstructor(new Instructor(iId, iName, spec)))
                            Console.WriteLine("--> Instructor added successfully!");
                        else
                            Console.WriteLine("--> Failed! Instructor ID already exists.");
                        break;

                    case "3":
                        Console.Write("Enter Course ID: ");
                        int cId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Course Title: ");
                        string cTitle = Console.ReadLine();
                        Console.Write("Enter Instructor ID for this course: ");
                        int instId = int.Parse(Console.ReadLine());

                        var instructor = manager.FindInstructor(instId);
                        if (instructor != null)
                        {
                            if (manager.AddCourse(new Course(cId, cTitle, instructor)))
                                Console.WriteLine("--> Course added successfully!");
                            else
                                Console.WriteLine("--> Failed! Course ID already exists.");
                        }
                        else
                        {
                            Console.WriteLine("--> Failed! Instructor not found.");
                        }
                        break;

                    case "4":
                        Console.Write("Enter Student ID: ");
                        int enrollSId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Course ID: ");
                        int enrollCId = int.Parse(Console.ReadLine());

                        if (manager.EnrollStudentInCourse(enrollSId, enrollCId))
                            Console.WriteLine("--> Student enrolled successfully!");
                        else
                            Console.WriteLine("--> Enrollment failed.");
                        break;

                    case "5":
                        Console.WriteLine("--- All Students ---");
                        if (manager.Students.Count == 0) Console.WriteLine("No students available.");
                        foreach (var s in manager.Students)
                            Console.WriteLine(s.PrintDetails());
                        break;

                    case "6":
                        Console.WriteLine("--- All Courses ---");
                        if (manager.Courses.Count == 0) Console.WriteLine("No courses available.");
                        foreach (var c in manager.Courses)
                            Console.WriteLine(c.PrintDetails());
                        break;

                    case "7":
                        Console.WriteLine("--- All Instructors ---");
                        if (manager.Instructors.Count == 0) Console.WriteLine("No instructors available.");
                        foreach (var inst in manager.Instructors)
                            Console.WriteLine(inst.PrintDetails());
                        break;

                    case "8":
                        Console.Write("Enter Student ID or Name: ");
                        string sSearch = Console.ReadLine();
                        var foundStudent = manager.Students.FirstOrDefault(s =>
                            s.StudentId.ToString() == sSearch ||
                            s.Name.Equals(sSearch, StringComparison.OrdinalIgnoreCase));

                        if (foundStudent != null)
                            Console.WriteLine(foundStudent.PrintDetails());
                        else
                            Console.WriteLine("--> Student not found.");
                        break;

                    case "9":
                        Console.Write("Enter Course ID or Title: ");
                        string cSearch = Console.ReadLine();
                        var foundCourse = manager.Courses.FirstOrDefault(c =>
                            c.CourseId.ToString() == cSearch ||
                            c.Title.Equals(cSearch, StringComparison.OrdinalIgnoreCase));

                        if (foundCourse != null)
                            Console.WriteLine(foundCourse.PrintDetails());
                        else
                            Console.WriteLine("--> Course not found.");
                        break;

                    case "10":
                        running = false;
                        Console.WriteLine("Exiting System... Goodbye!");
                        break;

                    case "11":
                        Console.Write("Enter Student ID: ");
                        int chkSId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Course ID: ");
                        int chkCId = int.Parse(Console.ReadLine());

                        bool isEnrolled = manager.IsStudentEnrolledInCourse(chkSId, chkCId);
                        Console.WriteLine(isEnrolled
                            ? "--> Yes, the student is enrolled in this course."
                            : "--> No, the student is NOT enrolled in this course.");
                        break;

                    case "12":
                        Console.Write("Enter Course Name: ");
                        string crsName = Console.ReadLine();
                        string instructorName = manager.GetInstructorNameByCourseName(crsName);
                        Console.WriteLine($"--> Instructor Name: {instructorName}");
                        break;

                    default:
                        Console.WriteLine("Invalid option! Please try again.");
                        break;
                }
            }
        }
    }
}