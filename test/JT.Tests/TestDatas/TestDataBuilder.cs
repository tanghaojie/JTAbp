using JT.EntityFrameworkCore;

namespace JT.Tests.TestDatas
{
    public class TestDataBuilder
    {
        private readonly JTDbContext _context;

        public TestDataBuilder(JTDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            //create test data here...
        }
    }
}