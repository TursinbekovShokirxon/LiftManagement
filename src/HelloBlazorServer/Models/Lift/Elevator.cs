using System.ComponentModel.DataAnnotations;

namespace LiftManagement.HelloBlazorServer.Models.Lift
{
    public class Elevator
    {
        [Key]
        public int Id { get; set; }
        public int CurrentFloor { get; set; } 
        public string Status { get; set; } 
    }
}
