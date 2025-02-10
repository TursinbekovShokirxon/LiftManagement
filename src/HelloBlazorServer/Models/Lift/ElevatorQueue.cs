using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiftManagement.HelloBlazorServer.Models.Lift
{
    public class ElevatorQueue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public int CurrentFloor { get; set; } 
        public DateTime RequestedAt { get; set; } 
    }
}
