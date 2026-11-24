namespace ContentService.Application.Queries.GetAllContent;

using MediatR;
using Domain.Interfaces;
using Dtos;

public class GetAllContentHandler : IRequestHandler<GetAllContentQuery, List<ContentDto>>
{
    private readonly IContentRepository _repository;

    public GetAllContentHandler(IContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ContentDto>> Handle(GetAllContentQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(ContentDto.From).ToList();
    }
}
