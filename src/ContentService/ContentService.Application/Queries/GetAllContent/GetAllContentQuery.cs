namespace ContentService.Application.Queries.GetAllContent;

using MediatR;
using Dtos;

public record GetAllContentQuery() : IRequest<List<ContentDto>>;
