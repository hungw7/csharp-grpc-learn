#nullable disable

using Grpc.Core;
using GrpcCodeLearn.Server.Models;

namespace GrpcCodeLearn.Server.Services;

public class DiscountService : Discount.DiscountBase
{
	private readonly List<DiscountModel> _discounts = new List<DiscountModel>
	{
		new DiscountModel { Id = "1", Amount = 40 },
		new DiscountModel { Id = "2", Amount = 50 },
		new DiscountModel { Id = "3", Amount = 60 },
	};

	public override Task<GetDiscountResponse> GetDiscount(GetDiscountRequest request, ServerCallContext context)
	{
		var response = _discounts.FirstOrDefault(x => x.Id == request.DiscountId);

		if (response is null)
			return Task.FromResult(new GetDiscountResponse());
		
		return Task.FromResult(new GetDiscountResponse()
		{
			Id = response!.Id,
			Amount = response.Amount
		});
	}
}