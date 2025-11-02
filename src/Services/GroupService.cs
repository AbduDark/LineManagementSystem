using LineManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LineManagementSystem.Services;

public class GroupService
{
    private readonly DatabaseContext _context;

    public GroupService(DatabaseContext context)
    {
        _context = context;
    }

    public List<LineGroup> GetGroupsByProvider(TelecomProvider provider)
    {
        return _context.LineGroups
            .Include(g => g.Lines)
            .Where(g => g.Provider == provider)
            .OrderByDescending(g => g.CreatedAt)
            .ToList();
    }

    public LineGroup? GetGroupById(int id)
    {
        return _context.LineGroups
            .Include(g => g.Lines)
            .FirstOrDefault(g => g.Id == id);
    }

    public LineGroup CreateGroup(LineGroup group)
    {
        group.CreatedAt = DateTime.Now;
        group.UpdatedAt = DateTime.Now;
        _context.LineGroups.Add(group);
        _context.SaveChanges();
        return group;
    }

    public void UpdateGroup(LineGroup group)
    {
        group.UpdatedAt = DateTime.Now;
        _context.LineGroups.Update(group);
        _context.SaveChanges();
    }

    public void DeleteGroup(int id)
    {
        try
        {
            // تنظيف الـ tracker قبل الحذف
            _context.ChangeTracker.Clear();
            
            // حذف المجموعة مباشرة باستخدام ExecuteDelete
            _context.LineGroups.Where(g => g.Id == id).ExecuteDelete();
        }
        catch (DbUpdateConcurrencyException)
        {
            // المجموعة محذوفة فعلاً
            _context.ChangeTracker.Clear();
        }
    }

    public void RenewGroupLicense(int groupId)
    {
        var group = GetGroupById(groupId);
        if (group != null)
        {
            group.RenewLicense();
            _context.SaveChanges();
        }
    }

    public void AddLineToGroup(int groupId, PhoneLine line)
    {
        var group = GetGroupById(groupId);
        if (group != null && group.CanAddMoreLines)
        {
            line.GroupId = groupId;
            line.CreatedAt = DateTime.Now;
            line.UpdatedAt = DateTime.Now;
            _context.PhoneLines.Add(line);
            _context.SaveChanges();
        }
        else
        {
            throw new InvalidOperationException("المجموعة وصلت للحد الأقصى من الخطوط (50 خط)");
        }
    }

    public void UpdateLine(PhoneLine line)
    {
        _context.ChangeTracker.Clear();
        line.UpdatedAt = DateTime.Now;
        _context.PhoneLines.Update(line);
        _context.SaveChanges();
    }

    public void DeleteLine(int lineId)
    {
        try
        {
            // استخدام ExecuteDelete عشان نحذف مباشرة من الداتابيز بدون tracking
            _context.PhoneLines.Where(l => l.Id == lineId).ExecuteDelete();
        }
        catch (DbUpdateConcurrencyException)
        {
            // السطر اتحذف فعلاً من مكان تاني
            _context.ChangeTracker.Clear();
        }
    }

    public Dictionary<TelecomProvider, int> GetGroupCountsByProvider()
    {
        return _context.LineGroups
            .GroupBy(g => g.Provider)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
