using TaskItem = GondolinWeb.Areas.Application.Models.Task;

namespace Gondolin.Tests
{
    public class TaskTest
    {
        private TaskItem testCase;

        public TaskTest()
        {
            testCase = new TaskItem();
        }

        [Fact]
        public void RequiredDate_DateTimeIsNow_ReturnsTrue()
        {
            // Arrange
            string expected = DateTime.Now.ToString("MM/dd/yyyy H:mm");
            // Act
            var result = testCase.CreationDate.ToString("MM/dd/yyyy H:mm");
            // Assert
            Assert.Equal(result, expected);
        }
    }
}