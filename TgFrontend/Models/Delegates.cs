using TgGateway.Models;

namespace TgFrontend.Models;

public delegate Task ButtonCallbackHandler(long chatId, IEnumerable<string> parameters);

public delegate Task MessageResponseHandler(
    long chatId,
    IEnumerable<string> parameters,
    TgMessage message);
