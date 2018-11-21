using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LaboratorioClinico.util
{
    public class PrintTicketLC
    {

        public void printTicket(DtoTicketPrint dtoTickerPrint, string namePrinter)
        {
            Ticket ticket = new Ticket();
            //ticket.HeaderImage = Resources.imagen_ticket; 

            ticket.AddHeaderLine("Tiendita ......" );
            ticket.AddHeaderLine("SUCURSAL ......" );
            ticket.AddHeaderLine("DIRECCION " );
            ticket.AddHeaderLine("CIUDAD " );
            ticket.AddHeaderLine("TEL: 545as4d5as4d" );
            ticket.AddSubHeaderLine("Caja 1 : TICKET 1");
            ticket.AddSubHeaderLine("CLIENTE: ");
            ticket.AddSubHeaderLine("EMPLEADO: ");
            ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());

            int num = 0 ; 
            foreach(DtoStudiesPrint study in dtoTickerPrint.Studies) {
                    num ++; 
                    ticket.AddItem("" + num , study.Name, study.Cost.ToString("C") );
            }
            
            ticket.AddTotal("SUBTOTAL", dtoTickerPrint.Subtotal.ToString("C"));
            ticket.AddTotal("DESCUENTO", dtoTickerPrint.Descuento.ToString("C"));
            ticket.AddTotal("TOTAL", dtoTickerPrint.Total.ToString("C"));
            ticket.AddTotal("", "" );
            ticket.AddTotal("RECIBIDO", dtoTickerPrint.Recibido.ToString("C"));
            ticket.AddTotal("POR PAGAR", dtoTickerPrint.PorPagar.ToString("C"));
            ticket.AddTotal("", "" );


            ticket.AddFooterLine("TIENDITA");
            ticket.AddFooterLine("CALIDAD Y CALIDEZ EN EL SERVICIO" );
            ticket.AddFooterLine("GRACIAS POR TU VISITA" );

            ticket.PrintTicket(namePrinter);
        }
        

    }
}
