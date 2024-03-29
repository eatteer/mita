﻿using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mita.DTOs;
using Mita.Models;

namespace Mita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MangasController : ControllerBase
    {
        private readonly MitaDatabaseContext _mitaContext;

        public MangasController(MitaDatabaseContext mitaContext)
        {
            _mitaContext = mitaContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Manga>>> FindAll()
        {
            List<Manga> mangas = await _mitaContext.Mangas.ToListAsync();
            return Ok(mangas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Manga>> FindById([FromRoute] int id)
        {
            // Check if manga exists
            Manga? manga = await _mitaContext.Mangas
                .Where(manga => manga.Id == id)
                .FirstOrDefaultAsync();

            if (manga == null) return NotFound();

            return Ok(manga);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Manga>> Create(CreateMangaDTO createMangaDTO)
        {
            // Validate if the request body is valid
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Create manga
            Manga manga = new Manga();
            manga.Name = createMangaDTO.Name;
            manga.MalUri = createMangaDTO.MalUri;
            manga.Status = createMangaDTO.Status;
            manga.Chapters = createMangaDTO.Chapters;
            manga.Volumes = createMangaDTO.Volumes;
            manga.Synopsis = createMangaDTO.Synopsis;
            manga.Score = createMangaDTO.Score;

            // Save manga in database
            await _mitaContext.Mangas.AddAsync(manga);
            int entriesWritten = await _mitaContext.SaveChangesAsync();

            if (entriesWritten == 0) Problem();

            return Created("/api/manga", manga);
        }

        [HttpPatch("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<Manga>> Update([FromRoute] int id, [FromBody] UpdateMangaDTO updateMangaDTO)
        {
            // Check if manga exists
            Manga? manga = await _mitaContext.Mangas
                .Where(manga => manga.Id == id)
                .FirstOrDefaultAsync();

            if (manga == null) return NotFound();

            // Update manga data
            manga.Name = updateMangaDTO.Name ?? manga.Name;
            manga.MalUri = updateMangaDTO.MalUri ?? manga.MalUri;
            manga.Status = updateMangaDTO.Status ?? manga.Status;
            manga.Chapters = updateMangaDTO.Chapters ?? manga.Chapters;
            manga.Volumes = updateMangaDTO.Volumes ?? manga.Volumes;
            manga.Synopsis = updateMangaDTO.Synopsis ?? manga.Synopsis;
            manga.Score = updateMangaDTO.Score ?? manga.Score;

            // Save manga changes in database
            int entriesWritten = await _mitaContext.SaveChangesAsync();

            if (entriesWritten == 0) return Problem();

            return Ok(manga);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            // Check if manga exists
            Manga? manga = await _mitaContext.Mangas
                .Where(manga => manga.Id == id)
                .FirstOrDefaultAsync();

            if (manga == null) return NotFound();

            // Remove manga from database
            _mitaContext.Mangas.Remove(manga);
            await _mitaContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
