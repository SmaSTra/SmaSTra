package de.tu_darmstadt.smastra.generator;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;

import java.lang.reflect.Type;
import java.util.List;

import de.tu_darmstadt.smastra.generator.elements.ProxyPropertyObj;
import de.tu_darmstadt.smastra.generator.extras.AbstractSmaSTraExtra;
import de.tu_darmstadt.smastra.markers.elements.config.ConfigurationElement;

/**
 * This is the abstract base for a SmaSTra serialization to a metadata object.
 * @author Tobias Welther
 */
public abstract class AbstractSmaSTraSerializer <T extends SmaSTraElement> implements JsonSerializer<T> {

    protected static final String TYPE_PATH = "type";
    protected static final String NAME_PATH = "name";
    protected static final String AUTHOR_PATH = "author";
    protected static final String MAIN_CLASS_PATH = "mainClass";
    protected static final String DISPLAY_PATH = "display";
    protected static final String DESCRIPTION_PATH = "description";

    protected static final String NEEDED_PERMISSIONS_PATH = "neededPermissions";
    protected static final String NEEDED_CLASSES_PATH = "needs";

    protected static final String CONFIG_PRE_PATH = "config";
    protected static final String CONFIG_KEY_PATH = "key";
    protected static final String CONFIG_DESCRIPTION_PATH = "description";
    protected static final String CONFIG_CLASS_TYPE_PATH = "classType";

    protected static final String PROXY_PROPERTIES_PATH = "proxyProperties";
    protected static final String PROXY_PROPERTIES_TYPE_PATH = "type";
    protected static final String PROXY_PROPERTIES_NAME_PATH = "name";
    protected static final String PROXY_PROPERTIES_METHOD_PATH = "method";

    protected static final String EXTRA_PATH = "extras";


    /**
     * The Type of the Serializer.
     */
    protected final String type;


    protected AbstractSmaSTraSerializer(String type) {
        this.type = type;
    }


    @Override
    public JsonObject serialize(T src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject obj = new JsonObject();
        obj.addProperty(TYPE_PATH, type);
        obj.addProperty(NAME_PATH, src.displayName);
        obj.addProperty(AUTHOR_PATH, "SmaSTraBasic");
        obj.addProperty(MAIN_CLASS_PATH, src.getElementClass().getCanonicalName());
        obj.addProperty(DISPLAY_PATH, src.getDisplayName());
        obj.addProperty(DESCRIPTION_PATH, src.getDescription());


        //Needed Permissions:
        JsonArray permsArray = new JsonArray();
        for(String perm : src.getAndroidPermissions()) permsArray.add(perm);
        obj.add(NEEDED_PERMISSIONS_PATH, permsArray);


        //Needs other classes:
        JsonArray others = new JsonArray();
        for(Class<?> clazz : src.getNeededClasses()) others.add(clazz.getCanonicalName());
        obj.add(NEEDED_CLASSES_PATH, others);


        //Write config if present:
        List<ConfigurationElement> config = src.getConfiguration();
        JsonArray array = new JsonArray();
        obj.add(CONFIG_PRE_PATH, array);

        if(config != null && !config.isEmpty()){
            for(ConfigurationElement element : config){
                JsonObject elementObj = new JsonObject();
                elementObj.addProperty(CONFIG_KEY_PATH, element.key());
                elementObj.addProperty(CONFIG_DESCRIPTION_PATH, element.description());
                elementObj.addProperty(CONFIG_CLASS_TYPE_PATH, element.configClass().getCanonicalName());

                array.add(elementObj);
            }
        }


        //Write the Proxy properties if present:
        JsonArray proxyArray = new JsonArray();
        obj.add(PROXY_PROPERTIES_PATH, proxyArray);
        List<ProxyPropertyObj> proxyProperties = src.getProxyProperties();
        if(!proxyProperties.isEmpty()){
            for(ProxyPropertyObj proxyProperty : proxyProperties){
                JsonObject proxyObj = new JsonObject();
                proxyObj.addProperty(PROXY_PROPERTIES_TYPE_PATH, proxyProperty.getProxyClass().getCanonicalName());
                proxyObj.addProperty(PROXY_PROPERTIES_METHOD_PATH, proxyProperty.getMethod().getName());
                proxyObj.addProperty(PROXY_PROPERTIES_NAME_PATH, proxyProperty.getProperty().name());

                proxyArray.add(proxyObj);
            }
        }

        //Write the Extras to the present:
        JsonArray extras = new JsonArray();
        obj.add(EXTRA_PATH, extras);
        for(AbstractSmaSTraExtra extra : src.getExtras()) extras.add(extra.serialize());

        return obj;
    }
}
