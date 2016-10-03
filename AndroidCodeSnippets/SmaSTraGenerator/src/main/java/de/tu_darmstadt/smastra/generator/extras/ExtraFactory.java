package de.tu_darmstadt.smastra.generator.extras;

import de.tu_darmstadt.smastra.markers.elements.extras.ExtraBroadcast;
import de.tu_darmstadt.smastra.markers.elements.extras.ExtraLibrary;
import de.tu_darmstadt.smastra.markers.elements.extras.ExtraService;

/**
 * Created by Toby on 03.10.2016.
 */

public class ExtraFactory {


    /**
     * Creates an Element from the Annotation passed.
     * @param element to create from.
     * @return the created element or null if failed.
     */
    public static AbstractSmaSTraExtra buildFromExtra(Object element){
        if(element == null) return null;

        //Broadcast:
        if(element instanceof ExtraBroadcast){
            ExtraBroadcast extraBroadcast = (ExtraBroadcast) element;
            return new NeedsBroadcast(extraBroadcast.clazz().getCanonicalName(), extraBroadcast.exported());
        }

        //Service:
        if(element instanceof ExtraService){
            ExtraService extraService = (ExtraService) element;
            return new NeedsService(extraService.clazz().getCanonicalName(), extraService.exported());
        }

        //Library:
        if(element instanceof ExtraLibrary){
            ExtraLibrary extraLibrary = (ExtraLibrary) element;
            return new NeedsLibrary(extraLibrary.libName());
        }


        return null;
    }


}
