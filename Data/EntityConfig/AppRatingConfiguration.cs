using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Entities;

namespace MovieTicketBooking.Data.EntityConfig
{
    public class AppRatingConfiguration : IEntityTypeConfiguration<AppRating>
    {
        public void Configure(EntityTypeBuilder<AppRating> builder)
        {
            builder.ToTable("AppRating");

            // Cấu hình cột "Id" làm khóa chính
            builder.HasKey(a => a.Id);

            // Định nghĩa cột "Id" thành "RatingId"
            builder.Property(a => a.Id)
                .HasColumnName("Id") // Đặt tên cột trong DB
                .ValueGeneratedOnAdd(); // Tự động tăng giá trị (Identity)

            // Định nghĩa cột "Rating" với giới hạn độ dài
            builder.Property(a => a.Rating)
                .HasColumnName("Rating") // Đổi tên cột thành "RatingValue"
                .HasMaxLength(50) // Giới hạn độ dài tối đa 50 ký tự
                .IsRequired(); // Bắt buộc nhập (NOT NULL)
        }
    }
}
