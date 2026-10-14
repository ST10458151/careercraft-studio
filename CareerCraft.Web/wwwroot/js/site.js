(() => {
  "use strict";
  const menu=document.getElementById("menuButton"),nav=document.getElementById("siteNav");
  if(menu && nav){
    menu.addEventListener("click",()=>{
      const open=nav.classList.toggle("open");
      menu.setAttribute("aria-expanded",String(open));
      menu.setAttribute("aria-label",open?"Close navigation":"Open navigation");
    });
    nav.querySelectorAll("a").forEach(a=>a.addEventListener("click",()=>{
      nav.classList.remove("open");menu.setAttribute("aria-expanded","false");menu.setAttribute("aria-label","Toggle navigation");
    }));
  }
  const year=document.getElementById("year");if(year)year.textContent=new Date().getFullYear();
  const form=document.getElementById("enquiryForm");
  if(!form)return;
  const select=document.getElementById("service"),summary=document.getElementById("selectedSummary"),notice=document.getElementById("formNotice");
  function setSummary(){
    const option=select.selectedOptions[0];
    summary.textContent=option?.dataset?.price ? option.dataset.name+" — R "+Number(option.dataset.price).toFixed(0)+" (draft price; confirmed before work begins)" : "Select a service to see its draft price.";
  }
  select.addEventListener("change",setSummary);setSummary();
  form.addEventListener("submit",e=>{
    e.preventDefault();
    if(!form.reportValidity()){notice.textContent="Please complete all required fields before continuing.";return;}
    const selected=select.selectedOptions[0];
    const input=id=>document.getElementById(id).value.trim();
    const recipient=form.dataset.contact;
    const body=[
      "Hello CareerCraft Studio,","",
      "I would like to enquire about the following service:","",
      "Service: "+selected.dataset.name,
      "Displayed draft price: R "+Number(selected.dataset.price).toFixed(0),
      "Full name: "+input("fullName"),
      "Reply email: "+input("email"),
      "Target role: "+input("targetRole"),
      "Vacancy link: "+(input("jobLink")||"Not provided"),
      "Career goals / support requested: "+(input("goals")||"Not provided"),
      "",
      "I understand that this is an enquiry only and that no payment or order has been placed.",
      "Please confirm the scope, turnaround time and next steps before I send any CV documents.",
      "",
      "Kind regards,",
      input("fullName")
    ].join("\n");
    const subject="CareerCraft Studio enquiry — "+selected.dataset.name;
    const mailto="mailto:"+recipient+"?subject="+encodeURIComponent(subject)+"&body="+encodeURIComponent(body);
    notice.textContent="Your email app should open now. Review the draft and press Send there. If it doesn't open, please contact "+recipient+".";
    window.location.href=mailto;
  });
})();
