import { acClearToasts, acFormHandler, acGetData, acInit, acPostData, acToastClient, acToastServer, getUrl } from './../global.js'



const nameInput = document.getElementById('ipName') as HTMLInputElement;
const emailInput = document.getElementById('ipEmail') as HTMLInputElement;
const topicSelect = document.getElementById('ddlTopic') as HTMLSelectElement;
const messageInput = document.getElementById('ipMessage') as HTMLTextAreaElement;
const agreeCheckbox = document.getElementById('chkAgree') as HTMLInputElement;



acInit([
    async () => acFormHandler('frmContact',await SubmitMessage) 
]);



async function SubmitMessage() {
    acClearToasts();
    console.log("check source map");
    const formData: Contact = {
        FromName: nameInput.value,
        Email: emailInput.value,
        Category: topicSelect.value,
        MessageText: messageInput.value,
        Origin: getUrl().fullUrl
    };

    const resp: APIResponse = await acPostData('/api/message/send', formData);
    acToastServer(resp);

}