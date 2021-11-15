using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Variants
{
    public enum EMP_TYPE
    {
        Salaried,
        Commission,
        DailyWage
    }

    public enum COMPARE_TYPE
    {
        Payout,
        EmpType,
        Name
    }
    public abstract class Employee
    {
        public Employee(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
        public string FirstName { get;set; }
        public string LastName { get; set; }
        public EMP_TYPE _employeeType { get; set; }
        public abstract decimal Payout(int months);
    }

    public class Salaried : Employee
    {
        public Salaried(string firstName, string lastName, decimal salary) : base(firstName, lastName)
        {
            Salary = salary;
            _employeeType = EMP_TYPE.Salaried;
        }
        public decimal Salary { get; set; }

        public override decimal Payout(int months)
        {
            return Salary * months;
        }
    }

    public class Comission : Employee
    {
        public Comission(string firstName, string lastName, decimal rate) : base(firstName, lastName)
        {
            CommissionRate = rate;
            _employeeType = EMP_TYPE.Commission;
        }
        public decimal CommissionRate { get; set; }
        public override decimal Payout(int months)
        {
            return CommissionRate * months;
        }
    }

    public class DailyEarner: Employee
    {
        public DailyEarner(string firstName, string lastName, decimal ratePerDay) : base(firstName, lastName)
        {
            RatePerDay = ratePerDay;
            _employeeType = EMP_TYPE.DailyWage;
        }
        public decimal RatePerDay { get; set; }
        public override decimal Payout(int months)
        {
            return RatePerDay * (months * 20);
        }
    }

    interface IEnumerateResource<out T>: IEnumerable<T>
    {
        T GetTopGrosser();
        IEnumerable<T> GetSortedByGross();
        IEnumerable<T> GetSortedByName();
        IEnumerable<T> GetSortedByType();
    }

    public class EmployeeCollection : IEnumerateResource<Employee>
    {
        List<Employee> _employees;

        public EmployeeCollection ()
        {
            _employees = new List<Employee>();
        }
        public void Add(Employee employee)
        {
            _employees.Add(employee);
        }
        public IEnumerator GetEnumerator()
        {
            return new EmployeeEnumerator(_employees.ToArray());
        }

        IEnumerator<Employee> IEnumerable<Employee>.GetEnumerator()
        {
            return new EmployeeEnumerator(_employees.ToArray());
        }

        public Employee GetTopGrosser()
        {
            return new SortedSet<Employee>(_employees, new CompensationComprarer(COMPARE_TYPE.Payout)).Max;
        }

        public IEnumerable<Employee> GetSortedByGross()
        {
            return new SortedSet<Employee>(_employees, new CompensationComprarer(COMPARE_TYPE.Payout));
        }

        public IEnumerable<Employee> GetSortedByType()
        {
            return new SortedSet<Employee>(_employees, new CompensationComprarer(COMPARE_TYPE.EmpType));
        }

        public IEnumerable<Employee> GetSortedByName()
        {
            return new SortedSet<Employee>(_employees, new CompensationComprarer(COMPARE_TYPE.Name));
        }
    }

    public class EmployeeEnumerator : IEnumerator<Employee>
    {
        Employee[] _employees;
        int pos = -1;

        public Employee Current => _employees[pos];

        object IEnumerator.Current => _employees[pos];

        public EmployeeEnumerator(Employee[] employees)
        {
            _employees = employees;
        }

        public void Reset()
        {
            pos = -1;
        }

        public bool MoveNext()
        {
            pos++;
            return pos < _employees.Length;
        }

        public void Dispose()
        {
            _employees = null;
        }
    }

    interface IResourceComparer<in T> : IComparer<T>
    {
    }

    // Some parts of code from https://stackoverflow.com/questions/62622078/how-to-sort-sortedset-by-value-that-can-be-duplicate
    public class CompensationComprarer : IResourceComparer<Employee>
    {
        COMPARE_TYPE _type;

        public CompensationComprarer(COMPARE_TYPE type)
        {
            _type = type;
        }

        public  int Compare([AllowNull] Employee x, [AllowNull] Employee y)
        {
            if (x == null) return y == null ? 0 : -1;

            int diff;
            switch (_type)
            {
                case COMPARE_TYPE.Payout: diff = x.Payout(1).CompareTo(y.Payout(1)); break;
                case COMPARE_TYPE.EmpType: diff = x._employeeType.CompareTo(y._employeeType); break;
                case COMPARE_TYPE.Name: diff = x.FirstName.CompareTo(y.FirstName); break;
                default: diff = x.Payout(1).CompareTo(y.Payout(1)); break;
            }

            // This is to show duplicates
            if (diff == 0)
            {
                return x.LastName.CompareTo(y.LastName);
            }

            return diff;
        }
    }

    
    class Program
    {
        static void Main(string[] args)
        {
            EmployeeCollection empCollection = new EmployeeCollection();

            var salary1 = new Salaried("Glenn", "Quagmire", 50000);
            var comission1 = new Comission("Peter", "Griffin", 15000);
            var daily1 = new DailyEarner("Joe", "Swanson", 850);

            var salary2 = new Salaried("Jennifer", "Lawrence", 70000);
            var comission2 = new Comission("Zoe", "Deschanel", 25000);
            var daily2 = new DailyEarner("Angelina", "Jolie", 1350);

            var salary3 = new Salaried("Ted", "Mosby", 25000);
            var comission3 = new Comission("Barney", "Stinson", 40000);
            var daily3 = new DailyEarner("Marshall", "Ericksen", 1050);

            var rand1 = new Comission("Steve", "Austin", 35000);

            empCollection.Add(salary1);
            empCollection.Add(comission1);
            empCollection.Add(daily1);

            empCollection.Add(salary2);
            empCollection.Add(comission2);
            empCollection.Add(daily2);

            empCollection.Add(salary3);
            empCollection.Add(comission3);
            empCollection.Add(daily3);
            empCollection.Add(rand1);

            Console.WriteLine("=== Listing all of employees ===");
 
            foreach (var employee in empCollection)
            {
                var e = (Employee)employee;
                Console.WriteLine($"{e.FirstName} {e.LastName}");
            }

            Console.WriteLine();
            Console.WriteLine("==== Employees sorted by their compensation from lowest to highest ===");
            foreach (var employee in empCollection.GetSortedByGross())
            {
                var e = (Employee)employee;
                Console.WriteLine($"{e.FirstName} {e.LastName} earns {e.Payout(1)}");
            }

            //Console.WriteLine();
            //Console.WriteLine("==== Employees sorted by their name ascending ===");
            //foreach (var employee in empCollection.GetSortedByName())
            //{
            //    var e = (Employee)employee;
            //    Console.WriteLine($"{e.FirstName} {e.LastName}");
            //}

            //Console.WriteLine();
            //Console.WriteLine("==== Employees sorted by their employee type ===");
            //foreach (var employee in empCollection.GetSortedByType())
            //{
            //    var e = (Employee)employee;
            //    Console.WriteLine($"{e.FirstName} {e.LastName} is a {e._employeeType}");
            //}

            // Added this just so I can try using a method with covariance
            //Console.WriteLine();
            //Console.WriteLine("=== TOP EARNER!! ===");
            //var topEarner = empCollection.GetTopGrosser();
            //Console.WriteLine($"{topEarner.FirstName} {topEarner.LastName} earning {topEarner.Payout(1)}");



            Console.ReadKey();
        }
    }
}
