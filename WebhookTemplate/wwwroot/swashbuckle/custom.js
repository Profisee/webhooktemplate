const pollDom = () => {
    const renderedMarkdownDivElements = document.getElementsByClassName("renderedMarkdown");

    if (renderedMarkdownDivElements.length) {
        let headingContent = `
            <p>
                This sample webhook provides a basic template for developers looking to create a webhook to use with Profisee's workflow engine.
                The API accepts and returns JSON.
            </p>

            <p>
                To test an operation, click the <strong>Try it out</strong> button to unlock the parameters. Enter the required information and then click Execute.
            </p>

            <p>
                Optionally, you can import the Swagger OpenAPI JSON directly into Postman using the following link:
            </p>

            <table>`;

        window.openApiUrls.map(urlObject => {
            headingContent += `
                <tr>
                    <td class="link-table-cell link-table-label-cell">${urlObject.name}</td>
                    <td class="link-table-cell"><a href="${urlObject.url}" target="_blank">${urlObject.url}</a></td>
                </tr>`;
        });

        headingContent += `
            </table>

            <p>
                For more information on Profisee custom development options including this web-based API and our SDK, visit the support site in any of the following areas:
            </p>

            <table>
                <tr>
                    <td class="link-table-cell link-table-label-cell">Documentation/Help</td>
                    <td class="link-table-cell"><a href="https://support.profisee.com/aspx/Software_Help" target="_blank">https://support.profisee.com/aspx/Software_Help</a></td>
                </tr>
                <tr>
                    <td class="link-table-cell link-table-label-cell">Knowledge Base</td>
                    <td class="link-table-cell"><a href="https://support.profisee.com/bloglist/list/00h00000000001E00aM" target="_blank">https://support.profisee.com/bloglist/list/00h00000000001E00aM</a></td>
                </tr>
                <tr>
                    <td class="link-table-cell link-table-label-cell">Downloads</td>
                    <td class="link-table-cell"><a href="https://support.profisee.com/Sys/document" target="_blank">https://support.profisee.com/Sys/document</a></td>
                </tr>
                <tr>
                    <td class="link-table-cell link-table-label-cell">Profisee Academy</td>
                    <td class="link-table-cell"><a href="https://support.profisee.com/lms" target="_blank">https://support.profisee.com/lms</a></td>
                </tr>
            </table>

            <p>
                Additional resources:
            </p>

            <table>
                <tr>
                    <td class="link-table-cell link-table-label-cell">Swagger/OpenAPI</td>
                    <td class="link-table-cell"><a href="https://swagger.io" target="_blank">https://swagger.io</a></td>
                </tr>
            </table>

            <p>
                For general support, visit the support web site at <a href="https://support.profisee.com" target="_blank">https://support.profisee.com</a>.
                To request help, submit a support ticket by visiting
                <a href="https://support.profisee.com/aspx/NewHelpTicket" target="_blank">https://support.profisee.com/aspx/NewHelpTicket</a>.
            </p>

            <p>
                <i>Have a suggestion to improve this API or other Profisee software?  Submit your idea at</i>
                <a href="https://support.profisee.com/idea" target="_blank">https://support.profisee.com/idea</a>
            </p>
        `;

        renderedMarkdownDivElements[0].innerHTML += headingContent;
    } else {
        setTimeout(pollDom, 500);
    }
};

(() => {
    pollDom();
})();
