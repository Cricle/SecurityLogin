using SecurityLogin.Redis.Finders;
using StackExchange.Redis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore.Services
{
    public class StudentIdCacheFinder : HashSetCacheFinder<string, int?>
    {
        public StudentIdCacheFinder(IDatabase database) : base(database)
        {
        }

        protected override Task<int?> OnFindInDbAsync(string identity)
        {
            return Task.FromResult<int?>(new Random().Next(0, 9999));
        }
        protected override TimeSpan GetCacheTime(string identity, int? entity)
        {
            return TimeSpan.FromSeconds(10);
        }
    }
    public class StudentCacheFinder : HashSetCacheFinder<string, Student>
    {
        public StudentCacheFinder(IDatabase database) : base(database)
        {
        }

        protected override Task<Student> OnFindInDbAsync(string identity)
        {
            var rand=new Random();
            return Task.FromResult<Student>(new Student
            {
                Id = rand.Next(1111, 9999),
                Name = Guid.NewGuid().ToString(),
                CarId = rand.Next(2222, 1111111),
                CreateTime = DateTime.Now,
            });
        }
        protected override TimeSpan GetCacheTime(string identity, Student entity)
        {
            return TimeSpan.FromSeconds(10);
        }
    }
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateTime { get; set; }

        public long CarId { get; set; }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj is Student s)
            {
                return s.Id == Id && s.Name==Name&&
                    s.CreateTime==CreateTime&&
                    s.CarId==CarId;
            }
            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var h = 17 * 31 + Id.GetHashCode();
                h = 17 * 31 + (Name?.GetHashCode() ?? 0);
                h = 17 * 31 + CreateTime.GetHashCode();
                h = 17 * 31 + CarId.GetHashCode();
                return h;
            }
        }
    }
}
