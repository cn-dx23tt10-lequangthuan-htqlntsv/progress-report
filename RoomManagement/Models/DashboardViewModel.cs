namespace RoomManagement.Models
{
    public class DashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int RentedRooms { get; set; }
        public int EmptyRooms { get; set; }
        public int TotalTenants { get; set; }

        public List<Contract> ExpiringContracts { get; set; } = new List<Contract>();
        public List<Room> AvailableRooms { get; set; } = new List<Room>();
    }
}