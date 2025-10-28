using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.Services.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Sube una imagen a Cloudinary.
        /// </summary>
        Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file);

        /// <summary>
        /// Elimina una imagen de Cloudinary por su publicId.
        /// </summary>
        Task DeleteImageAsync(string publicId);

        /// <summary>
        /// Valida que el archivo sea una imagen válida.
        /// </summary>
        bool ValidateImage(IFormFile file);
    }
}