// This file is part of SayedHa.Blackjack.
//
// SayedHa.Blackjack is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SayedHa.Blackjack is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with SayedHa.Blackjack.  If not, see <https://www.gnu.org/licenses/>.
namespace SayedHa.Blackjack.Api {
	public static class HandEndpoints {

		public static void MapHandEndpoints(this IEndpointRouteBuilder routes) {
			var group = routes.MapGroup("/hand").WithTags("hand");

			group.MapGet("/getnewhand", () => {
				return 1;
			});

			group.MapGet("/receivecard", () => {

			});
		}
		


	}
}
