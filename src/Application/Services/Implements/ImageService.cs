using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Serilog;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Exceptions;

namespace TiendaProyecto.src.Application.Services.Implements
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public ImageService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public bool ValidateImage(IFormFile file)
        {
            // Validar tamaño
            if (file.Length > MaxFileSize)
            {
                Log.Warning("Archivo demasiado grande: {Size} bytes", file.Length);
                return false;
            }

            // Validar MIME type
            if (!_allowedMimeTypes.Contains(file.ContentType.ToLower()))
            {
                Log.Warning("MIME type no permitido: {MimeType}", file.ContentType);
                return false;
            }

            // Validar extensión
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                Log.Warning("Extensión no permitida: {Extension}", extension);
                return false;
            }

            return true;
        }

        public async Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file)
        {
            if (!ValidateImage(file))
            {
                throw new BadRequestAppException("Archivo de imagen no válido");
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "tienda-ucn/products",
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                Log.Error("Error al subir imagen: {Error}", uploadResult.Error.Message);
                throw new Exception($"Error al subir imagen: {uploadResult.Error.Message}");
            }

            Log.Information("Imagen subida exitosamente: {PublicId}", uploadResult.PublicId);
            return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
        }

        public async Task DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Result != "ok")
            {
                Log.Warning("No se pudo eliminar la imagen: {PublicId}", publicId);
                throw new Exception($"Error al eliminar imagen: {result.Result}");
            }

            Log.Information("Imagen eliminada exitosamente: {PublicId}", publicId);
        }
    }
}