using MessagePack;
using MessagePack.Resolvers;
using SecurityLogin.Redis.Annotations;
using SecurityLogin.Redis.Converters;
using SecurityLogin.Redis.Finders;
using SecurityLogin.Redis.Finders;
using StackExchange.Redis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore.Services
{
    public class StudentIdCacheFinder : ListCacheFinder<string, int?>
    {
        public StudentIdCacheFinder(IDatabase database) : base(database)
        {
        }

        protected override Task<int?> OnFindInDbAsync(string identity)
        {
            return Task.FromResult<int?>(new Random().Next(0, 9999));
        }
        protected override TimeSpan? GetCacheTime(string identity, int? entity)
        {
            return TimeSpan.FromSeconds(10);
        }
    }
    public class StudentCacheFinder : ListCacheFinder<string, Student>
    {
        public StudentCacheFinder(IDatabase database) : base(database)
        {
        }
        public async Task<long> GetSizeAsync(string identity)
        {
            var key = GetEntryKey(identity);
            var data = await Database.ListRangeAsync(key);
            return data.Sum(x => x.Length());
        }
        protected override Task<Student> OnFindInDbAsync(string identity)
        {
            var rand=new Random();
            return Task.FromResult(new Student
            {
                Id = rand.Next(1111, 9999),
                Name = Student.CreateLargeText(10240),
                Name1 = Student.CreateLargeText(10240),
                CarId = rand.Next(2222, int.MaxValue),
                CreateTime = DateTime.Now,
            });
        }
        protected override TimeSpan? GetCacheTime(string identity, Student entity)
        {
            return TimeSpan.FromSeconds(10);
        }
    }
    public class MessagePackCacheFinder : CacheFinderBase<string, Student>
    {
        private static readonly Type type = typeof(Student);

        private static readonly MessagePackSerializerOptions opt = MessagePackSerializerOptions.Standard.WithResolver(TypelessObjectResolver.Instance)
            .WithCompression( MessagePackCompression.Lz4Block);

        public MessagePackCacheFinder(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; }
        public async Task<long> GetSizeAsync(string identity)
        {
            var key = GetEntryKey(identity);
            var data = await Database.StringGetAsync(key);
            return data.Length();
        }
        protected override async Task<Student> CoreFindInCacheAsync(string key, string identity)
        {
            var str = await Database.StringGetAsync(key);
            if (str.HasValue)
            {
                return (Student)MessagePackSerializer.Deserialize(type, str, opt);
            }
            return null;
        }

        protected override Task<Student> OnFindInDbAsync(string identity)
        {
            var rand = new Random();
            return Task.FromResult(new Student
            {
                Id = rand.Next(1111, 9999),
                Name = Student.CreateLargeText(70240),
                Name1 = Student.CreateLargeText(70240),
                CarId = rand.Next(2222, int.MaxValue),
                CreateTime = DateTime.Now,
            });
        }

        protected override Task<bool> WriteCacheAsync(string key, string identity, Student entity, TimeSpan? caheTime)
        {
            var bf= MessagePackSerializer.Serialize(entity, opt);
            return Database.StringSetAsync(key, bf,caheTime);
        }
    }
    public class Student
    {
        public int Id { get; set; }

        [CacheValueConverter(typeof(GzipStringCacheValueConverter))]
        public string Name { get; set; }

        [CacheValueConverter(typeof(GzipStringCacheValueConverter))]
        public string Name1 { get; set; }

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
        public static string CreateLargeText(int size)
        {
            var s = new StringBuilder(size*36);
            for (int i = 0; i < size; i++)
            {
                s.Append(Guid.NewGuid().ToString());
            }
            return s.ToString();
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
