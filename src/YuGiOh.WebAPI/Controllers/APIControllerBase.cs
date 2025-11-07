using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace YuGiOh.WebAPI.Controllers
{
    /// <summary>
    /// Serves as the base class for all API controllers in the application.
    /// </summary>
    /// <remarks>
    /// This abstract base controller provides shared functionality for derived controllers,
    /// including access to the MediatR <see cref="ISender"/> instance used to send commands and queries
    /// through the mediator pipeline.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class APIControllerBase : ControllerBase
    {
        /// <summary>
        /// The MediatR sender instance used to dispatch requests (commands and queries)
        /// to their respective handlers in the application layer.
        /// </summary>
        private readonly ISender _sender;

        /// <summary>
        /// Provides access to the MediatR <see cref="ISender"/> for use by derived controllers.
        /// </summary>
        protected ISender Sender => _sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="APIControllerBase"/> class.
        /// </summary>
        /// <param name="sender">The MediatR sender instance used for request dispatching.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the <paramref name="sender"/> dependency is null.
        /// </exception>
        protected APIControllerBase(ISender sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }
    }
}
