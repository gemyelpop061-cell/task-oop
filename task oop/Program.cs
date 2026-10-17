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

    public class CourseEnrollment
    {
        public Course Course { get; set; }
        public double Grade { get; set; }

        public CourseEnrollment(Course course, double grade = 0.0)
        {
            Course = course;
            Grade = grade;
        }
    }

    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public Instructor Instructor { get; set; }
        public int MaxCapacity { get; set; }

        public Course(int courseId, string title, Instructor instructor, int maxCapacity)
        {
            CourseId = courseId;
            Title = title;
            Instructor = instructor;
            MaxCapacity = maxCapacity;
        }

        public string PrintDetails()
        {
            string instructorName = Instructor != null ? Instructor.Name : "No Instructor Assigned";
            return $"Course ID: {CourseId} | Title: {Title} | Instructor: {instructorName} | Max Capacity: {MaxCapacity}";
        }
    }

    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public List<CourseEnrollment> Enrollments { get; set; }

        public Student(int studentId, string name, int age)
        {
            StudentId = studentId;
            Name = name;
            Age = age;
            Enrollments = new List<CourseEnrollment>();
        }

        public bool Enroll(Course course)
        {
            if (course == null) return false;

            if (Enrollments.Any(e => e.Course.CourseId == course.CourseId))
            {
                return false;
            }

            Enrollments.Add(new CourseEnrollment(course));
            return true;
        }

        public bool AssignGrade(int courseId, double grade)
        {
            var enrollment = Enrollments.FirstOrDefault(e => e.Course.CourseId == courseId);
            if (enrollment == null) return false;

            enrollment.Grade = grade;
            return true;
        }

        public double CalculateAverageGrade()
        {
            if (Enrollments.Count == 0) return 0.0;
            return Enrollments.Average(e => e.Grade);
        }

        public string GetAcademicStatus(int partTimeThreshold, int fullTimeThreshold)
        {
            if (Enrollments.Count == 0) return "Not Enrolled";
            if (Enrollments.Count <= partTimeThreshold) return "Part-Time Student";
            if (Enrollments.Count >= fullTimeThreshold) return "Full-Time Advanced Student";
            return "Regular Student";
        }

        public string PrintDetails(int partTimeThreshold, int fullTimeThreshold)
        {
            string courseList = Enrollments.Count > 0
                ? string.Join(", ", Enrollments.Select(e => e.Course.Title))
                : "None";

            return $"Student ID: {StudentId} | Name: {Name} | Age: {Age} | GPA/Avg: {CalculateAverageGrade():F1} | Status: [{GetAcademicStatus(partTimeThreshold, fullTimeThreshold)}] | Enrolled: [{courseList}]";
        }
    }

    public class StudentManager
    {
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Instructor> Instructors { get; set; } = new List<Instructor>();

        public int PartTimeThreshold { get; set; } = 2;
        public int FullTimeThreshold { get; set; } = 3;

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

        public bool UpdateStudent(int studentId, string newName, int newAge)
        {
            var student = FindStudent(studentId);
            if (student == null) return false;

            if (!string.IsNullOrWhiteSpace(newName)) student.Name = newName;
            if (newAge > 0) student.Age = newAge;
            return true;
        }

        public bool DeleteStudent(int studentId)
        {
            var student = FindStudent(studentId);
            if (student == null) return false;

            return Students.Remove(student);
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

            if (student == null || course == null) return false;

            int currentEnrolledCount = Students.Count(s => s.Enrollments.Any(e => e.Course.CourseId == courseId));
            if (currentEnrolledCount >= course.MaxCapacity)
            {
                return false;
            }

            return student.Enroll(course);
        }

        public bool AssignGradeToStudent(int studentId, int courseId, double grade)
        {
            var student = FindStudent(studentId);
            if (student == null) return false;

            return student.AssignGrade(courseId, grade);
        }

        public bool IsStudentEnrolledInCourse(int studentId, int courseId)
        {
            var student = FindStudent(studentId);
            if (student == null) return false;

            return student.Enrollments.Any(e => e.Course.CourseId == courseId);
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

        public Student GetMostActiveStudent()
        {
            return Students.OrderByDescending(s => s.Enrollments.Count).FirstOrDefault();
        }

        public string GetMostPopularCourse()
        {
            if (Courses.Count == 0) return "No courses available.";

            var popular = Courses
                .Select(c => new
                {
                    Title = c.Title,
                    Count = Students.Count(s => s.Enrollments.Any(e => e.Course.CourseId == c.CourseId))
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            if (popular == null || popular.Count == 0)
                return "No enrollments yet.";

            return $"{popular.Title} ({popular.Count} students enrolled)";
        }

        public string GetTopInstructor()
        {
            if (Instructors.Count == 0) return "No instructors available.";

            var top = Instructors
                .Select(i => new
                {
                    Name = i.Name,
                    CourseCount = Courses.Count(c => c.Instructor != null && c.Instructor.InstructorId == i.InstructorId)
                })
                .OrderByDescending(x => x.CourseCount)
                .FirstOrDefault();

            if (top == null || top.CourseCount == 0)
                return "No courses assigned to instructors yet.";

            return $"{top.Name} (Teaches {top.CourseCount} course(s))";
        }

        public List<Instructor> GetUnassignedInstructors()
        {
            return Instructors.Where(i => !Courses.Any(c => c.Instructor != null && c.Instructor.InstructorId == i.InstructorId)).ToList();
        }

        public List<Student> SearchStudentsMultiField(string query)
        {
            return Students.Where(s =>
                s.StudentId.ToString() == query ||
                s.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                s.Age.ToString() == query
            ).ToList();
        }

        public void DisplayStudentTranscript(int studentId)
        {
            var student = FindStudent(studentId);
            if (student == null)
            {
                Console.WriteLine("--> Student not found!");
                return;
            }

            Console.WriteLine("================ ACADEMIC TRANSCRIPT ================");
            Console.WriteLine($"Student ID   : {student.StudentId}");
            Console.WriteLine($"Name         : {student.Name}");
            Console.WriteLine($"Age          : {student.Age}");
            Console.WriteLine($"Academic GPA : {student.CalculateAverageGrade():F2}");
            Console.WriteLine($"Status       : {student.GetAcademicStatus(PartTimeThreshold, FullTimeThreshold)}");
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("Enrolled Courses, Grades & Instructors:");

            if (student.Enrollments.Count == 0)
            {
                Console.WriteLine("No courses enrolled yet.");
            }
            else
            {
                foreach (var item in student.Enrollments)
                {
                    string instructorName = item.Course.Instructor != null ? item.Course.Instructor.Name : "N/A";
                    Console.WriteLine($" -> {item.Course.Title} | Grade: {item.Grade} | Instructor: {instructorName}");
                }
            }
            Console.WriteLine("=====================================================");
        }

        public void DisplaySystemAnalytics()
        {
            Console.WriteLine("=================== SYSTEM ANALYTICS ===================");
            Console.WriteLine($"Total Students       : {Students.Count}");
            Console.WriteLine($"Total Courses        : {Courses.Count}");
            Console.WriteLine($"Total Instructors    : {Instructors.Count}");

            double avgCourses = Students.Count > 0 ? Students.Average(s => s.Enrollments.Count) : 0;
            Console.WriteLine($"Avg Courses/Student   : {avgCourses:F1}");
            Console.WriteLine($"Most Popular Course  : {GetMostPopularCourse()}");

            var topStudent = GetMostActiveStudent();
            string topStudentName = topStudent != null ? $"{topStudent.Name} ({topStudent.Enrollments.Count} courses)" : "None";
            Console.WriteLine($"Most Active Student  : {topStudentName}");
            Console.WriteLine($"Top Instructor       : {GetTopInstructor()}");
            Console.WriteLine($"Unassigned Insts     : {GetUnassignedInstructors().Count}");
            Console.WriteLine("========================================================");
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
                Console.WriteLine("8. Find Student (by ID, Name, or Age)");
                Console.WriteLine("9. Find Course (by ID or Title)");
                Console.WriteLine("10. Update Student Info");
                Console.WriteLine("11. Delete Student");
                Console.WriteLine("12. Check if Student is Enrolled in Specific Course");
                Console.WriteLine("13. Get Instructor Name by Course Name");
                Console.WriteLine("14. Display Student Academic Transcript & GPA");
                Console.WriteLine("15. Assign Grade to Student for Course");
                Console.WriteLine("16. Show Unassigned Instructors");
                Console.WriteLine("17. Display System Analytics & Executive Report");
                Console.WriteLine("18. Configure Dynamic System Thresholds");
                Console.WriteLine("19. Exit");
                Console.Write("Enter your choice (1-19): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        int sId = ReadInteger("Enter Student ID: ");
                        Console.Write("Enter Student Name: ");
                        string sName = Console.ReadLine();
                        int sAge = ReadInteger("Enter Student Age: ");

                        if (manager.AddStudent(new Student(sId, sName, sAge)))
                            Console.WriteLine("--> Student added successfully!");
                        else
                            Console.WriteLine("--> Failed! Student ID already exists.");
                        break;

                    case "2":
                        int iId = ReadInteger("Enter Instructor ID: ");
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
                        int cId = ReadInteger("Enter Course ID: ");
                        Console.Write("Enter Course Title: ");
                        string cTitle = Console.ReadLine();
                        int instId = ReadInteger("Enter Instructor ID for this course: ");
                        int capacity = ReadInteger("Enter Max Course Capacity Limit: ");

                        var instructor = manager.FindInstructor(instId);
                        if (instructor != null)
                        {
                            if (manager.AddCourse(new Course(cId, cTitle, instructor, capacity)))
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
                        int enrollSId = ReadInteger("Enter Student ID: ");
                        int enrollCId = ReadInteger("Enter Course ID: ");

                        if (manager.EnrollStudentInCourse(enrollSId, enrollCId))
                            Console.WriteLine("--> Student enrolled successfully!");
                        else
                            Console.WriteLine("--> Enrollment failed (Course reached capacity limit or student already enrolled).");
                        break;

                    case "5":
                        Console.WriteLine("--- All Students ---");
                        if (manager.Students.Count == 0) Console.WriteLine("No students available.");
                        foreach (var s in manager.Students)
                            Console.WriteLine(s.PrintDetails(manager.PartTimeThreshold, manager.FullTimeThreshold));
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
                        Console.Write("Enter Student Search Keyword (ID, Name, or Age): ");
                        string query = Console.ReadLine();
                        var results = manager.SearchStudentsMultiField(query);

                        if (results.Count > 0)
                        {
                            foreach (var res in results)
                                Console.WriteLine(res.PrintDetails(manager.PartTimeThreshold, manager.FullTimeThreshold));
                        }
                        else
                        {
                            Console.WriteLine("--> No students found matching your query.");
                        }
                        break;

                    case "9":
                        Console.Write("Enter Course ID or Title: ");
                        string cSearch = Console.ReadLine();
                        var foundCourse = manager.Courses.FirstOrDefault(c =>
                            c.CourseId.ToString() == cSearch ||
                            c.Title.IndexOf(cSearch, StringComparison.OrdinalIgnoreCase) >= 0);

                        if (foundCourse != null)
                            Console.WriteLine(foundCourse.PrintDetails());
                        else
                            Console.WriteLine("--> Course not found.");
                        break;

                    case "10":
                        int upSId = ReadInteger("Enter Student ID to Update: ");
                        Console.Write("Enter New Name: ");
                        string upName = Console.ReadLine();
                        int upAge = ReadInteger("Enter New Age: ");

                        if (manager.UpdateStudent(upSId, upName, upAge))
                            Console.WriteLine("--> Student updated successfully!");
                        else
                            Console.WriteLine("--> Student not found.");
                        break;

                    case "11":
                        int delSId = ReadInteger("Enter Student ID to Delete: ");
                        if (manager.DeleteStudent(delSId))
                            Console.WriteLine("--> Student deleted successfully!");
                        else
                            Console.WriteLine("--> Student not found.");
                        break;

                    case "12":
                        int chkSId = ReadInteger("Enter Student ID: ");
                        int chkCId = ReadInteger("Enter Course ID: ");

                        bool isEnrolled = manager.IsStudentEnrolledInCourse(chkSId, chkCId);
                        Console.WriteLine(isEnrolled
                            ? "--> Yes, the student is enrolled in this course."
                            : "--> No, the student is NOT enrolled in this course.");
                        break;

                    case "13":
                        Console.Write("Enter Course Name: ");
                        string crsName = Console.ReadLine();
                        string instructorName = manager.GetInstructorNameByCourseName(crsName);
                        Console.WriteLine($"--> Instructor Name: {instructorName}");
                        break;

                    case "14":
                        int transcriptSId = ReadInteger("Enter Student ID for Transcript: ");
                        manager.DisplayStudentTranscript(transcriptSId);
                        break;

                    case "15":
                        int gSId = ReadInteger("Enter Student ID: ");
                        int gCId = ReadInteger("Enter Course ID: ");
                        double grade = ReadDouble("Enter Grade Value: ");

                        if (manager.AssignGradeToStudent(gSId, gCId, grade))
                            Console.WriteLine("--> Grade assigned successfully!");
                        else
                            Console.WriteLine("--> Failed! Student is not enrolled in this course.");
                        break;

                    case "16":
                        Console.WriteLine("--- Unassigned Instructors ---");
                        var unassigned = manager.GetUnassignedInstructors();
                        if (unassigned.Count == 0) Console.WriteLine("All instructors are currently assigned to courses.");
                        foreach (var inst in unassigned)
                            Console.WriteLine(inst.PrintDetails());
                        break;

                    case "17":
                        manager.DisplaySystemAnalytics();
                        break;

                    case "18":
                        manager.PartTimeThreshold = ReadInteger("Enter max courses limit for Part-Time Status: ");
                        manager.FullTimeThreshold = ReadInteger("Enter min courses limit for Full-Time Status: ");
                        Console.WriteLine("--> System status thresholds updated dynamically!");
                        break;

                    case "19":
                        running = false;
                        Console.WriteLine("Exiting System... Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid option! Please try again.");
                        break;
                }
            }
        }

        private static int ReadInteger(string prompt)
        {
            int value;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Invalid input! Please enter a valid number: ");
            }
            return value;
        }

        private static double ReadDouble(string prompt)
        {
            double value;
            Console.Write(prompt);
            while (!double.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Invalid input! Please enter a valid numeric value: ");
            }
            return value;
        }
    }
}
