using Microsoft.EntityFrameworkCore;
using Stardust.Database.Context;
using Stardust.Database.Data;

namespace Stardust.Database.CRUD
{
    public class StardustCRUD
    {
        private readonly StardustContext _context;

        public StardustCRUD(StardustContext context)
        {
            _context = context;
        }

        #region Report
        public async Task<List<Report>> GetReportsAsync()
        {
            return await _context.Reports.ToListAsync();
        }
        public async Task UpdateReportAsync(Report report)
        {
            _ = _context.Reports.Update(report);
            _ = await _context.SaveChangesAsync();
        }
        public async Task DeleteReportAsync(Report report)
        {
            _ = _context.Reports.Remove(report);
            _ = await _context.SaveChangesAsync();
        }

        public async Task<List<Report>?> GetReportsByUserIdAsync(string? userId)
        {
            return await _context.Reports.Where(r => r.UserId == userId).ToListAsync();
        }

        public async Task<List<Report>?> GetReportsByMachineIdAsync(int id)
        {
            return await _context.Reports.Where(r => r.MachineId == id).ToListAsync();
        }

        public async Task<List<Report>> GetReportsByMachineHostname(string? hostname)
        {
            List<Data.Report> reports = new();
            if (hostname == null)
                return reports;
            Machine? machine = await _context.Machines.SingleOrDefaultAsync(m => m.Hostname == hostname);
            if (machine == null || _context.Reports == null)
                return reports;
            reports = await _context.Reports.Where(r => r.MachineId == machine.Id).OrderByDescending(r => r.CreatedAt).ToListAsync();
            return reports;
        }
        public async Task CreateReportAsync(Report report)
        {
            try
            {
                List<Report>? reports = await GetReportsAsync();
                int newId = reports.LastOrDefault()?.Id + 1 ?? 1;
                report.Id = newId;
                _ = _context.Reports.Add(report);
                _ = await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }

        }
        #endregion

        #region Machine
        public async Task CreateMachineAsync(Machine machine)
        {
            _ = _context.Machines.Add(machine);
            _ = await _context.SaveChangesAsync();
        }
        public async Task<List<Data.Machine>> GetMachinesAsync()
        {
            return await _context.Machines.ToListAsync();
        }
        public async Task UpdateMachineAsync(Machine machine)
        {
            try
            {
                _context.Entry(machine).State = EntityState.Modified;
                _context.Entry(machine).Property("Id").IsModified = false;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }
        }

        public async Task DeleteMachineAsync(Machine machine)
        {
            _ = _context.Machines.Remove(machine);
            _ = await _context.SaveChangesAsync();
        }
        public async Task<Machine?> GetMachineByHostnameAsync(string? hostname)
        {
            if (hostname == null)
                return null;
            return await _context.Machines.SingleOrDefaultAsync(m => m.Hostname == hostname);
        }
        public async Task<Machine?> GetMachineByIdAsync(int? id)
        {
            if (id == null)
                return null;
            return await _context.Machines.SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Machine?> GetMachineByMacAddressAsync(string macAddress)
        {
            return await _context.Machines.SingleOrDefaultAsync(m => m.MacAddress == macAddress);
        }
        #endregion

        #region User
        public async Task CreateUserAsync(StardustUser user)
        {
            _ = _context.StardustUsers.Add(user);
            _ = await _context.SaveChangesAsync();
        }

        public async Task<List<StardustUser>> GetUsersAsync()
        {
            return await _context.StardustUsers.ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(StardustUser user)
        {
            try
            {
                _context.StardustUsers.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task DeleteUserAsync(StardustUser user)
        {
            _ = _context.StardustUsers.Remove(user);
            _ = await _context.SaveChangesAsync();
        }

        public async Task<StardustUser?> GetUserByNameAsync(string? name)
        {
            if (name == null)
                return null;
            return await _context.StardustUsers.SingleOrDefaultAsync(u => u.Name == name);
        }

        public async Task<StardustUser?> GetUserByEmailAsync(string? email)
        {
            if (email == null)
                return null;
            return await _context.StardustUsers.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<StardustUser?> GetUserByAvatarAsync(string avatar)
        {
            return await _context.StardustUsers.SingleOrDefaultAsync(u => u.Avatar == avatar);
        }

        public async Task<StardustUser?> GetUserByRoleAsync(StardustUserRole role)
        {
            return await _context.StardustUsers.SingleOrDefaultAsync(u => u.Role == role);
        }
        #endregion

        #region Scripts
        public async Task<List<Script>> GetScriptsAsync()
        {
            return await _context.Scripts.ToListAsync();
        }

        public async Task<Script?> GetScriptByNameAsync(string? name)
        {
            if (name == null)
                return null;
            return await _context.Scripts.SingleOrDefaultAsync(s => s.Name == name);
        }

        public async Task<bool> AddScriptAsync(Script script)
        {
            if (await GetScriptByNameAsync(script.Name) != null)
                return false;

            List<Script> scripts = await GetScriptsAsync();
            int newId = scripts.LastOrDefault()?.Id + 1 ?? 1;
            script.Id = newId;
            _ = _context.Scripts.Add(script);
            _ = await _context.SaveChangesAsync();

            return await GetScriptByNameAsync(script.Name) != null;
        }

        public async Task UpdateScriptAsync(Script script)
        {
            _ = _context.Scripts.Update(script);
            _ = await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteScriptAsync(Script script)
        {
            _ = _context.Scripts.Remove(script);
            _ = await _context.SaveChangesAsync();

            return await GetScriptByNameAsync(script.Name) == null;
        }

        public async Task<Script?> GetScriptByIdAsync(int id)
        {
            return await _context.Scripts.SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Script>?> GetScriptsByCategoryAsync(ScriptCategory category)
        {
            return await _context.Scripts.Where(s => s.Category == category).ToListAsync();
        }

        public async Task<List<Script>?> GetScriptsByConcernedPortAsync(int port)
        {
            return await _context.Scripts.Where(s => s.ConcernedPorts.Contains(Convert.ToString(port))).ToListAsync();
        }
        #endregion

        #region Tasks
        public async Task<List<TaskTodo>?> GetAllTasksTodoAsync()
        {
            return await _context.TasksTodo.ToListAsync();
        }

        public async Task<TaskTodo?> GetTaskTodoByIdAsync(int id)
        {
            return await _context.TasksTodo.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByUserIdAsync(string userId)
        {
            return await _context.TasksTodo.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByAssignedToIdAsync(int assignedToId)
        {
            return await _context.TasksTodo.Where(t => t.AssignedToId == assignedToId).ToListAsync();
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByDeviceIdAsync(int? deviceId)
        {
            if (deviceId == null)
                return null;
            return await _context.TasksTodo.Where(t => t.DeviceId == deviceId).ToListAsync();
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByStatusAsync(TodoTaskStatus status)
        {
            return await _context.TasksTodo.Where(t => t.Status == status).ToListAsync();
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByEmergencyAsync(TodoTaskEmergency emergency)
        {
            return await _context.TasksTodo.Where(t => t.Emergency == emergency).ToListAsync();
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByDueDateAsync(DateTime dueDate)
        {
            return await _context.TasksTodo.Where(t => t.DueDate == dueDate).ToListAsync();
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByCompletedDateAsync(DateTime completedDate)
        {
            return await _context.TasksTodo.Where(t => t.CompletedDate == completedDate).ToListAsync();
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByCreatedDateAsync(DateTime createdDate)
        {
            return await _context.TasksTodo.Where(t => t.CreatedDate == createdDate).ToListAsync();
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByLastUpdatedDateAsync(DateTime lastUpdatedDate)
        {
            return await _context.TasksTodo.Where(t => t.LastUpdatedDate == lastUpdatedDate).ToListAsync();
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByTitleAsync(string title)
        {
            return await _context.TasksTodo.Where(t => t.Title == title).ToListAsync();
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByDescriptionAsync(string description)
        {
            return await _context.TasksTodo.Where(t => t.Description == description).ToListAsync();
        }

        public async Task<List<TaskTodo>?> GetTaskTodoByReportAsync(Data.Report report)
        {
            return await _context.TasksTodo.Where(t => t.ReportId == report.Id).ToListAsync();
        }

        public async Task<bool> AddTaskTodoAsync(TaskTodo task)
        {
            try
            {
                _ = _context.TasksTodo.Add(task);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _ = await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateTaskTodoAsync(TaskTodo task)
        {
            try
            {
                _ = _context.TasksTodo.Update(task);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _ = await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteTaskTodoAsync(TaskTodo task)
        {
            try
            {
                _ = _context.TasksTodo.Remove(task);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _ = await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region GlobalScan

        public async Task<List<GlobalScan>> GetAllGlobalScansAsync()
        {
            return await _context.GlobalScans.ToListAsync();
        }

        public async Task<bool> AddGlobalScanAsync(GlobalScan globalScan)
        {
            try
            {
                globalScan.Id = (await GetAllGlobalScansAsync()).LastOrDefault()?.Id + 1 ?? 1;
                globalScan.Launched_At = DateTime.Now;
                _ = _context.GlobalScans.Add(globalScan);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }
        }

        public async Task UpdateGlobalScanAsync(GlobalScan globalScan)
        {
            try
            {
                _context.Entry(globalScan).State = EntityState.Modified;
                _context.Entry(globalScan).Property("Id").IsModified = false;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }
        }

        public async Task<GlobalScan?> GetGlobalScanByIdAsync(int id)
        {
            return await _context.GlobalScans.SingleOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<GlobalScan>?> GetGlobalScanByInitiatorAsync(string initiator_name)
        {
            return await _context.GlobalScans.Where(g => g.Initiated_By == initiator_name).ToListAsync();
        }

        public async Task<GlobalScan?> GetNewestLastScanAsync()
        {
            return await _context.GlobalScans.OrderByDescending(g => g.Ended_At).FirstOrDefaultAsync();
        }

        #endregion
    }
}
