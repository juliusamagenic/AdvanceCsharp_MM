using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct CommissionDetails
{
    public string name;
    public decimal ratePerCommission;
}

public abstract class Contractual
{
    public enum ContractType 
    {
        NotContractual = -1,
        PerHour,
        PerDay,
        PerMonth,
        PerCommission,
    };

    protected int hoursRendered;
    protected List<CommissionDetails> _commissions;
    protected ContractType _contractType;

    public Contractual ()
    {
        _commissions = new List<CommissionDetails>();
    }

    public void RenderWork(int hours)
    {
        hoursRendered += hours;
    }

    public void RenderWork(CommissionDetails commission)
    {
        _commissions.Add(commission);
    }

    public abstract decimal Payout();
}
public class Employee : Contractual
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime BirthDate { get; set; }
    internal decimal Salary { get; set; }

    public Employee()
    {
        _contractType = ContractType.NotContractual;
    }

    ~Employee() { }
    public virtual void Work() 
    {
        RenderWork(8);
    }

    public override decimal Payout()
    {
        switch (_contractType)
        {
            case ContractType.PerDay:
                {
                    var salaryPerDay = Salary / 20;
                    var dayWorked = (hoursRendered / 8);

                    if (dayWorked > 0)
                        return dayWorked * salaryPerDay;

                    return 0;
                }
            case ContractType.PerHour:
                {
                    var salaryPerHour = Salary / 20 / 8;

                    if (hoursRendered > 0)
                        return hoursRendered * salaryPerHour;

                    return 0;
                }
            case ContractType.PerMonth:
                {
                    var dayWorked = (hoursRendered / 8);

                    // A month is 20 work days
                    if (dayWorked > 19)
                        return Salary;

                    return 0;
                }

            case ContractType.PerCommission:
                {
                    var total = 0M;

                    foreach (var commission in _commissions)
                    {
                        total += commission.ratePerCommission;
                    }

                    return total;
                }
        }

        return Salary;
    }

    public void setContract(ContractType ct)
    {
        _contractType = ct;
    }
}


public class Consultant : Employee
{ 
    public string ProjectName { get; set; }
    public void Work(CommissionDetails commission)
    {
        _commissions.Add(commission);
    }
}
public class QualityEngineer : Employee
{
    public string TestingTool { get; set; }
    public override void Work()
    {
        RenderWork(8);
    }
}
public class Developer : Employee
{
    public string ProgrammingLanguage { get; set; }
    public override void Work()
    {
        RenderWork(8);
    }
}

namespace Employee_Extend
{
    class Program
    {
        static void Main(string[] args)
        {
            // All employee here has a monthly rate given in their salary.
            // The rate per hour and day is computed based on the monthly rate which is the salary

            // q1 is strict and wants his hours be meaningful. He insists that he got paid by the hour.
            QualityEngineer q1 = new QualityEngineer();
            q1.FirstName = "Bernardo";
            q1.LastName = "Reyes";
            q1.Salary = 30000;
            q1.setContract(Contractual.ContractType.PerHour);

            // Work for 40 hours
            q1.Work();
            q1.Work();
            q1.Work();
            q1.Work();
            q1.Work();

            Console.WriteLine($"{q1.FirstName} {q1.LastName} got paid for ${q1.Payout()}");

            // d1 is a chill person. He works per day and got paid at the end of the day.
            Developer d1 = new Developer();
            d1.FirstName = "Pablo";
            d1.LastName = "Escoban";
            d1.Salary = 25000;
            d1.setContract(Contractual.ContractType.PerDay);

            // Work for 2 days
            d1.Work();
            d1.Work();

            Console.WriteLine($"{d1.FirstName} {d1.LastName} got paid for ${d1.Payout()}");

            Developer d2 = new Developer();
            d2.FirstName = "Ernesto";
            d2.LastName = "Iglesias";
            d2.Salary = 20000;
            d2.setContract(Contractual.ContractType.PerMonth);

            // Work for 10 days only because found another opportunity outside the country.
            // And our contract strictly states that a month must be rendered before paying.
            d2.Work();
            d2.Work();

            // d2 didn't get paid. You should have stayed for a month. or at least read the contract before signing.
            Console.WriteLine($"{d2.FirstName} {d2.LastName} got paid for ${d2.Payout()}");

            // c1 is confident on his work. he does not go by time consume by the work.
            // c1 goes with the commission per work done. such chad.
            Consultant c1 = new Consultant();
            c1.FirstName = "Juan";
            c1.LastName = "Dela Cruz";
            c1.setContract(Contractual.ContractType.PerCommission);

            c1.Work(new CommissionDetails
            {
                name = "First Commission",
                ratePerCommission = 20000,
            });

            c1.Work(new CommissionDetails
            {
                name = "Second Commission",
                ratePerCommission = 1000,
            });

            c1.Work(new CommissionDetails
            {
                name = "Third Commission",
                ratePerCommission = 5000,
            });

            Console.WriteLine($"{c1.FirstName} {c1.LastName} got paid for ${c1.Payout()}");

            Console.ReadKey();
            Console.WriteLine("Press Any Key to Exit...");
            
        }
    }
}
