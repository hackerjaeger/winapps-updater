﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "102.0b6";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/102.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "7af2ce41043a33d4b7640f9527505787a1041e9647fbb6b492bfef55a474124ee79b2920de4f3e3fcb790a296d26b828096d60b02f3ab36257504eb9d5bc37f6" },
                { "af", "4cd0f120a76396e2517bfab2ed9bf559189cc3e62e60907e3263e03bf3eaaad9be733ab7157b4ab6522fe678824e2649ba5f50cd557e69617a52a330f46e8b06" },
                { "an", "febbd9e76330efa4bba467ba2c7d82c46813d922e609b199f7ddb6bcdba11094a0f9b96a43d11d909c1357bdf02a5d438b59c7b103e1bc82e1ca7dec0c2531f5" },
                { "ar", "3702f6e3d1f8c8e12afa5269344655eef76ac7d4b2dbeebac0cdfc8be4b0b2f0d9dc67749cc80ca71f767ddcf3d3534f1056a3d7cfc267b31de5c233a0b54642" },
                { "ast", "ff7ed55c6483b670ec29b3017abc8fac9075e830024ac1903b2af65d0179f8267c6641fe5c7d48dccd7cc988e90e3565829163bbbc1aeca03fcc6db80782822e" },
                { "az", "53d62927e3f69e2fd46550393e7c5e2cf47ace637e2690986112df231645adac6ad7d13b3158ad7a9f6d0b6691b59e610ab561b161d88db680ee38e23bf28a58" },
                { "be", "99c4ba77627621c6984280e108f1cc8564ae2e6dfd7802ac910907d3f91d5fee09ee738a9e3f22619da462fac7e4d854ceb899909d31d0996bb2bfd83341e63a" },
                { "bg", "5ccc811ef8a63561882f828f8baebacaa217d4816585fc821bf420cd4b12c584c62d1ce587475ff7e5d631626e2618abe628dbc53739dd08920148783efbea44" },
                { "bn", "ca44fd192c531200945d82bd4c11e3a1cc6c0d65cbc15fcabc7dfe4de265fa98114d979e57f1cf6b05adc8b40b02abc1a49bd6815e64ce708c92f4bc5d2ecb7f" },
                { "br", "c86bcbbe85fc23150ca7086e069b17e313730edadbeec5fe26784a716b5808ca4d462eb2d2be3f2a0c61a642ad8004e2ac13d7c6c8dba71fed1e30d4b5ed273f" },
                { "bs", "e72cf90e046a9dc29118873c262f46896b2df6e36968598a224c1e37aa4d7bf7f1c28828a1639fd6ff434af5f37e7eb130834074a9ecfea65b3135085102cb6e" },
                { "ca", "fe613b7dd08f411a8815b2baa895628c18240353b5e8d7cac19040701d31c382122a0de371d776bbe014708328c9b2170e561d69bfb6ed53cbfec89cfa7adc43" },
                { "cak", "e2d77f6ae6cee89c2269c42b8a06435877becc80a322d3ae6b2085b9102bd08501ea696e3866244181d2a92781f7c2331c3f5e45afa5792ccaa2da98a54d1bc4" },
                { "cs", "8674403f894248ddf3ad606669255d57f0e2a89ede69a08d3f59c2a727aad8962964cf6d63123c303865b1d5094dccc44154c1bbf44318fd0a6f09152aad603d" },
                { "cy", "e5801404d2630558e79f6ad7b425ae7e0b496c17c6cd87e01ecebad35ebf7af0f458ac37f4eb98c3c71cb49becbc08495c460dbdbf912cc8f781efd0ab6a40dd" },
                { "da", "264c74db0bc3944b83d1c2e5e1a88d08e75169fbfc8bf969df545350d69c6d3e0fb1a7bf2d41d88f3eab679ae12caa2eba45711106026ca5ae21c90a0c28a9a5" },
                { "de", "fb6a9c0fc766e7f95d80c4dc894f270c78e633a36a6c2e7125add3151977984e22824adc2cc488f0eec49914d3f214291b15ee130555b7197b65268f17728e95" },
                { "dsb", "6d9a5c7ecd75fa5d6bc81998814515e71c5637cd98a967afcd10ac6fe5b3e1de5dd6016886c9fd674ac9de386f6d09af1f7cb12de2c941b7149fe941bdea9730" },
                { "el", "0667b5cd18213c081e588613a557135fa0d087baae04148492a5a91dbcf723880a447cda68255efcd5b0ed36d011a56b6d0da0216c613b2e518f9e45e4e7b0f2" },
                { "en-CA", "d915603772491b83551f2ebcb109a6192647175e8ee1cc55b1485bb2a122da3ed652d343b399ed898fbe8e68200d8bd3aa7d4b27adef00d70a3912c9972aecad" },
                { "en-GB", "bf52448f45a6b1941364d14d41b4922e5a1ac7753a0094c6eb787f62a6382cea3340d61134c0f2cfb00c3d5155a9a1f03689b3442e401e443995076df80950cc" },
                { "en-US", "71d5ada962bdd9e8479efa6ac3480e6fc1ed70fa887ff3c163be61c1b6d08f91bf7ddbab54c236e82ed6d510b8c1e3b930c8819657a9e3403dc40612a1a03ac6" },
                { "eo", "6c419cb79e19d667ec29037fd45177f1318544b32fe067404dbe93a3051800c0076bbbf12be5b20377805300497037a8e66a21d44b62d299df1167e89eaab426" },
                { "es-AR", "2223181a6660d18c1e67aa800fe52161a6f048eafd6795f011138c36ca7ab65d36742488e186230302119ea1951c397c67a0f854ce1237a9b30b306607b2bfd9" },
                { "es-CL", "376f8c9555bfd11adbc7341aac44be19e4c75de4eae56a598b2eda4fc132c361be97768759e73e006e202bbe8b8c6c89cda8a6f55b26fbeeda8c35ec6268ae36" },
                { "es-ES", "afbf099aeb48bad89040836060abd7a5879155ad0ffd31d644f12d9ba83017b6ec872b6f94d8d731ae4a77bf69a6dcb22dbf1d753ea3ac2bd8c521ab976c83af" },
                { "es-MX", "a464b7ff7d7e98121db0d800792179b3a63cbd4680904740e283776edc7af5da068c2b9ce2e68c1c3a9f3a401f57c8314f45479b50b2eb575748cb5382e50b7b" },
                { "et", "2401fdd16343e0a760058a4bbccfd6139602001ac1ec8ec6615803b3fbfdc6fe69f80b7ba92fee092fcb0fcce279682d45d82e6c7683f544b351efd5dee372fb" },
                { "eu", "5c7687e7bd187b0ba45df15900b475b95a9fb74012dfb7d887eab56bc9631032b15f5acbc252387ea4e1520658f99a6de1e9994d6e825ea5cea41bf806d53a94" },
                { "fa", "d090cbb305e113568100e9184833803b6b16ced6c987c6f6f15b52032b4b4339a2c3c6b73d754c3dcb3ead37600a9bb784df9f30c5b79e221942978c254213d6" },
                { "ff", "a74c29649c431060b9f5c5823176f33a0eed7da82ff4dad4bdf3ed81a16edd84590d4e2d5f3756fcb1f513e82f6a824ae74359425366f87ab3a188654e4370dd" },
                { "fi", "f4ddbcecc654cc553ef1774e72730d6d50c61171b8731f9a3605443c91eb3922504b03e0f438c27d5db7e7456734fd65455e7712d8c8bb1ca2530e78f851b548" },
                { "fr", "90defdc255840a4b5c5b24002c4d3ca562f9374552fa491b18615f0f8911224c72e95daf08b9bee08318b8bfd025adb21978dac78e76014c14377e0d92368291" },
                { "fy-NL", "33890bdba9494173ea52cc859e485534c4d512f0ef2ca93f2ddca9e0622642d57fdcc0ca2b9525854ce6c7172baa0a0ce60abd6c6014f1ffcb968cccf75ff08f" },
                { "ga-IE", "6ce2edd98be6b2fbb20bd25a35faaf9ef5f0c6c905d7d075af5050a4dace4727702c8585b737026a6926a4725f32a651c237f2647a697ed1dd626b3d4329e3a8" },
                { "gd", "7c8f33f68031142a8a98710d1bcadba7259b912f72c1a05e27beeb2054a4f8621a07e45b3748a2213ac7c18a0693803b5463807a1e18c5399840f21d2ffb3644" },
                { "gl", "54211cdaf13c719b4e3ca9112e93686000ab636661c4ca868e5fa931e2994c31e1255ceac20b1584f08aa3f103077b866c9f3fd7c71cc720caa49930dbedda5b" },
                { "gn", "0b11be0b76154272411306a5b1b3a0f5876c064523bf05a3f445cc973c58fb06b8051752513782d6a8407667a8790311d996285bd7ef80618a00748ec72e0f06" },
                { "gu-IN", "770e2766b3d3be9cba642702489acf14c9bac2985c84030018be2e04c8e3e4a38993c838ff5523060eafcc25775ba871c69677c56fba793508f6fd1d835a73e7" },
                { "he", "d9befab9c7661866c56da7b9883f387c7914356682866e70b61d85210627d60e18bb38fa214e2f85b257cd6212f9f04464e9d9532d019eb1bc71b95d1078c76a" },
                { "hi-IN", "79442084ea899287a24a1ef495f14882bafbecb2f67f9e74ed1da252f04131acdebb823ce22f2a14bccdd8f3a2e0d34b9d7040857f559ae01c85e2851403be3f" },
                { "hr", "3e65daf8223de904a66d03326e89422d56c92bdaf575b9a201f411079094e6d4795b39a25e0ab69e4fb5957a75c0440e66a7474040ff2a77ab66d4f82b321bf6" },
                { "hsb", "c7adb9a210fb98e1baa727eb2e2072d7c694b987e38fd7880c5fb9774ed95fbed816648b0f5fad36aa620e9954f8c032fb5fb80e0b8416ec62c6e389f09a2fa0" },
                { "hu", "ea200a285e006f27d28285938626b95bae271ee05a86faf8f0676688c0d782218faaa3c2f9fb3ed7a7db284d9c9e6f242dd52cff8527bbda0e8c14a53bdc76a1" },
                { "hy-AM", "f4fe81f589421f1c89be50f58ac52fb4708ef394fff210dd07b9e35a84608222e17db761f20e99f7172a9fa8ef92733f0360b189f3a7ebf15e405ac5569c9aaf" },
                { "ia", "3ecc06bf15646cd4b3242595ad35ac69675abcedf868677a0ed0a5f26f6d52c0db8db93383eb3df73084010e75bdfb6049c3f14b7d6193e927ce0d364cc2a0b3" },
                { "id", "2c6447e4a31f0738158af5d21d841f069652b5087646f4fccf98c7a525e4ae6081d971151e16c61459cab79d6b901d16b21ea2f1f3ac9313265ac4fa9ddb8649" },
                { "is", "85a1a737696d878588ab460b49b2df6aa594528eb2b029219900c341a3291dbc21a0adbcbf8ee510919ebdd5c10ff813cb58833616a631a27c7b9bb1e6ccedc7" },
                { "it", "796d244c0b22a10de68020da90306fa9f1802a1d7c40919ee07205934a8f1aad2e5d7746974e3dfd5f876fc6f4e86f0b5c8d78152542a50f2199e47466516859" },
                { "ja", "f9876f977b31e1602d210972f2c74309b17879b2521cc31c7eaf5af814ac744f19bf17304f1ce4e4592ae82ac0bafce6be37a7b3e2803606deaa34e566176d77" },
                { "ka", "6d7ae2aeec94208e596b47a674db4092cf7e8c47b70473aca1fce5e9adf2d933653ae665da99e3f5073ff960bedbbd4d70566b87f05bd7fa9412d0192d583040" },
                { "kab", "62dfe04c1c179a56006b33fd1ce33aa7146f8ccb295ba14824f8909c63cb2ede7661f354a7627ca4c595cc412d417590ac814dc0747d507c5aa70b011cc313bf" },
                { "kk", "6b93df15de9af52e06b99a7dd6e014e69753397e892df58e90a9e9c5be98830a2b2b8ceb2d9be7c4136c3ea8792956d6555d36980ca50a7e8dc97b9e073a27b9" },
                { "km", "ee7f8b3e41c5b7220c83e3c9ca633e48de0f00d2eca57e4ffaa2803f607de55d7308f1bf9c5e4bc1f5d3437c6343e25ba28c2f802691997d2da80d79410b8c8b" },
                { "kn", "760383ea644fed0b122e8a34f5efa84c0e53d8416fbe45cbad3ac7266f36b0488d7227fad584336f025d4d36eeba1edd680e02d9fee472907501d69ce7925c60" },
                { "ko", "4ef0b547342e7e55488d4dd8c6b3a3d6394bcd4a4a597ab0f884b6dc1df2d91ccb3bf4b01e8ac6111606c11529745b1f0ac3fcea6306e8228796b778aab07999" },
                { "lij", "e213e737aeef36ab2e7b7522cdb94f01dd4830e89c345ab2521a9f77b8ec95dee602ae6db4ab7f0b60ec1fda9ede7145f6f032987f7fead7c94413503a876c1e" },
                { "lt", "c3736bd9e94f7e4c328a8c64aebdc1fe88be2d1752153d4c6f8474a40ca5c290aa5e7835998b15a5ed6e25d09b251fe9d68b0d831e1ce18d0d55ab3edece58f4" },
                { "lv", "b88e9a995a763cd30d0bbf445298280dd19e5be075337bf9b3fb04f93924eea5e9c40d2dd9270b96c7826b28ddec9326470aa55496508839b7f6af673e19a087" },
                { "mk", "8598d096725d858d9501e73e56f5fa6e65ae95e460046bdaf7230cfdde091cc0bd2189d1eec922968f6f02c6f8084abb0ec8e619bc558fcedfea9309a2581fe0" },
                { "mr", "4bdeb30da4dec3af5128385ed1f375f353f88816653d0ef09bae8c3e2de2fcd580ba543421a47375873be759df7c016638258a2782dab40d2523973dd159d06d" },
                { "ms", "cedce974b041e3b76c6d97ab74e50f7b43832afc92a069c15a33b66bd3f074756e10d6982279fb8f4bb2025b0d6c7af08d07c391e3a7a261be7e716fec9e8193" },
                { "my", "f5174138d2085359ea0aa741088ed7d6634f8989a52db2c4eacc4b5a047a2d1e7b3727e8b45800abbe2a8cbd8339e520d567b41e65a2a895b3d29d3ecdda93ae" },
                { "nb-NO", "bb398fedaec48ea3cba86d8fd97ca0c3be99fb99d76d8c0a692fbe6bd7e248f875354c97c495b231009dc0823d0d81ebffcf2def91cf828529b72403db85d146" },
                { "ne-NP", "633628d354cb7e267d960243d8311b26deb724066b55b47cbbe440b9bbce2c9bbf22fbaab16fa219ee9a757b029313fee0a0e1933a60b69d156cb145bebd0e69" },
                { "nl", "7364420bbddd1d9851a1c17cee9638374c52b6c2e9c531077960fd5a256d0f506f939866fbde98f4514b02a9b0989b0a51a531b627506e1323bdd476c513cd4a" },
                { "nn-NO", "7a78fd46d10febc1433963ed8ee02c72e50bf4b1ef65aa0db8f5630633169da815794e43c16d7fdba2dbd4d073fd5b98cf0711de03c004c3fd5edca075387e6c" },
                { "oc", "fa07c4447b2dddb744ccdea6e19bf3b8ab80e23d6d44911da3a68f4faac11d2fd9ee60732553c4ed09f79b5d832aa5df4c81b15647c93bab451d367b89edef25" },
                { "pa-IN", "a159ea474bcdc66d27baef1b9fdc3f1ab670b32f6b3fd3da9399969aec3d8d0cd5fd52d66ba2e1f0b58f1f108627c73e6847d50a1a322313da61eb6d6283afbc" },
                { "pl", "8373d9accb1c2862d5cafdf261e6b3290d0923f3d2d52d148ce066d8d4442d1d0a90a01f1f6f6d5932dc7fd121cd8f35dd08ad01101e1eb6464e0afc63330c01" },
                { "pt-BR", "38db0ae92e9c12666dfd4ec4451925490a705b7c80bf24860ebb3c1bf866e7618a1a68f58895a489b7b9af794e03e4f1ce406dc8494d889142f7cdaf1f2c910a" },
                { "pt-PT", "caca40ce86104355cacf09ec35023b1b5204c3b52118a92dad1213e9eb3a117fb9b6d7b72333d560767e1e685dd7221aa5a941aac454bcd70952c726d9ef8f36" },
                { "rm", "659bebc322993784dca51fdd092ea7abe3a5b0f085338f8e69c664a63d30b8bea4290625629c9031b728a02ad9ce5556326236d18332fe19efe7a22b4f25cc0b" },
                { "ro", "56bf431a2bb02cb5fcc53e34d9fa9d0c974ead7df88ff9a46dcf4927bb77586af3f0ac2c9bc9834e72a0523a32882f84fcd0138985769fa236041a0b78cdd48f" },
                { "ru", "4b21ff3a008af8de973548a423b235cc41f0ee885317609ebff071e7e12e28c6390de9afd5959dcf3d15a8e8d5f15644d4ea7277bf26c2802f1acd7085e1558d" },
                { "sco", "d3059e0ff90d9a5c68b6a7a4022ab077f3398b89d62d240cc641e42e7d32181eb9f9d97ad37b0cddda0e4a48c444035ed58a582432fec714fcdd0300edbc81e0" },
                { "si", "c0eac751493c859c5a760619e232be5fb86d947da518699305ed8a943abdafb44a9742085fb2a00e2ac11d303ef1a5045215acc281a7155ba521549c8b1e76b4" },
                { "sk", "cd72dec39d430e09e4c0ae0a8e8b8f7c332adb1069c6b61ffb2b8fcbcc02e60a2b8c4614b63915c2449d6d4bfca6b814c9618f08f8727502e2bbc96943a44b09" },
                { "sl", "86a3b6ba39e18c4c63d7c60213d7404ace42e9928029eb7cb6a2aeaf79bfe14440d62a09a9dfc787aa99b11426a5b233510f581270d6b988db048f0d26ce72cd" },
                { "son", "e348dea1d8ee08ccd79af63dadda96824fd510da2f50fb74efe9e8a74f0bda8e30a8be6030d4f20fcafe5fbdab5eea904153269ef2bdcb28e38f4818e76478ef" },
                { "sq", "cbec0a25616d7dc40718485a30f09e5ab6651d31eb4b8a20ca481b8574ca986d58d17a30f6c385217d631a6209e693dd72c4cf4b11af4cf38693ce0327f3b13e" },
                { "sr", "ba7b8fffea71c2550cbec4510b3843bcf00de3c1d90f5c1391d2ee4a47e8ced2c98dd2dbac75e78ba479f35ad528b0a43ddbacb06e8c6148aa7ec02543829ac1" },
                { "sv-SE", "41e2b02e6d7197b2c011ccb280b69daf3044246a3ed61191b54279eeaed5b8be262bc3222c044e343074d050d9bf01c72ce4fa619abe33404fa5d9f2a2de6a6a" },
                { "szl", "ac6c6797e45b8b60fa43071e4a718e640535d650b9b418960010c780ddf3d24b64dc621f5a0df15bb243dbcaf1001c67eab11c2c7218abc7e1c53c4879106646" },
                { "ta", "18173abc810abc4c72d8a348c96cae91e1de3b8d7acfb2bd099da28198877014ddf9f9cd6cbb3817f2a24b0bde6efd790edee108b16fb142bffb9c2aab91ee25" },
                { "te", "8aa42ba0c5ea89cac025e4db5b5e84d2a7c1b03a83d1027fe484262c3f50b43afa9b398edcbb767dea28a7ae377bb13e95e972dcb8cc81612b9b7b0362b56f6a" },
                { "th", "f2f9972f73261d1587a51a0c17f1f26c950e72ba9200fe02ef8c170ce77c20406d86182cc3a51f7fb8a95bece04d10f245a3bb021e9890f22a8ac2e6c7173a05" },
                { "tl", "12ec55457a611454dd6ec821f3499f860a5777ab8ceed61d8d21cd02acbf3cb11a1c7a20d51364d2574847536e00c816e0d515f90df902d01967a680742c7a2c" },
                { "tr", "3e4fbcf0de92a890533fd174fac67fedaf3b08fbed16ffd2a5aabb4f55b6137e488491d1de789fd72f161c35641431fd4ff73245788178c07d6460690c5dcf65" },
                { "trs", "806a4833ef406b674dd9f26e0c1664defa1fb56e54cc39c7fdb0bcc3bf8c7e59710a87367a4bd5a07213c33608bb649f59a5eba16ff78ff6f318fd10789ad4c3" },
                { "uk", "d84dbae4303f05abac2e18359239f7f0fecb01f98e1107c6a724a84e0ab2492bf488264e99fd0949a1e4dcef06debc756ed81f2cc0c2bafcee68f036209ecff3" },
                { "ur", "3df028e1b2e244f279ff801af7e765a5172013ba86e423e7bdd1b5ce42676c55f0b82fda5641fe73c748b654d85f732c287fe6ab8000de7a4e6aab6b246e008f" },
                { "uz", "8fb411fbc10179790f27bb6a8f23164d77e5ba97d5964598b16e583b2ff659fc571af04309cf1668df520e47498c580cd724e332cccf8651f156616bfa512a18" },
                { "vi", "4149d267a0a97a028c0204cb706c3d2a187aa846e41eec35185d550588dcc727d095c35c8dfc3b5d6466af0eb135299aa9d36b09b7f573e9a44759282f236dcb" },
                { "xh", "7ecedc72e1743fc312bab62bbcdf0a56bc99b4b0571f5f00bdba6a92b692b0b6ad846fd26a22fa01e0deaf82f80716d9b4728dd401021e14a3efced82bf1a82f" },
                { "zh-CN", "056730bea1e405e8010bb2226154dd64b664dd8b5bb8abf2aed243f3837e1924ef1a183acf5b038787b3c2de93ced5313aa1f5907ed203bc946691746b8270c8" },
                { "zh-TW", "f8392f530ce74b5d67230f04b68fe5454cd380fa6d23e6cb167e18a2ebca7f4595021e8677ae55a2f9638b993a55ac0d8bacca0f0f41d33954b0b77daf216765" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/102.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "d234fc04233d53d07a4815b7a0adae40c9c4f139dda1c37305f254de1d27128dce942cfa29d6e19b2b262d3788e75e388743581cb7c11e6e19b060f3ea272198" },
                { "af", "ac4b3caf796bb30e496d79019032f155794a502eef0f88c6cc8358fefdb0b87da682008b5bdaa41207524bfd21b7df520c74ad6ad1adf6692fce973ecc24dcb6" },
                { "an", "1c5223bc4fe2672342aa0ad59632ff0549b438e27b8982166e6045fae3240d68572dde0fed59e6f51f63fbac458c16f8cee77d1dcd3653e2e991e0d32ed91a0d" },
                { "ar", "38e231d550ce0a2f060b306a8094a295cf2c24f50ad3536ec2500ffde2461401082b8061a6a1d8cff08bc7b7eae6616b35a7184d08b6dea025d1beec9de782e1" },
                { "ast", "d3e5b2869e2f66b874769de6fb2394050633f3d1aa4b9b43d3d57eb400a1c5e4a58e4414181adbb758b98b93fba7a43a7e554137489ee4bbd748b7182fc35cb6" },
                { "az", "db4523d7a251fe65c9d2b274bcaceb18d5660c7eedb0f4904a0f3e720b8d18ec96e6ff04028a663daff2bb919cdf27a8cb8a1e013b5ed0d18714b0719b74189d" },
                { "be", "9b9bbe791f0827236e52a2a22bb137e1f700b5ce154b48278fcd063ed99230383b32faa690f3085477396fb4dd06a32e4b1eb9d00a2adefaed86b030b38ac2a8" },
                { "bg", "61d8cd8385dcc5cfd20601fa58a54aa02001f4757cffe3bbaa26bc651b702226c66ffa8e0fecb1fd54e768d5cd799d4268f8db4303c90390dcc0e23d70326a15" },
                { "bn", "456973a3211a6c1c57dd9d7ffb9766497aa69858baf3b2145513cf0aa549297ef8275e53955400ba6646ffd04ae2629fb989fdf06457623b04f3105f6490aa7d" },
                { "br", "29379adf945ddb3757d084c58b8b14f46492760085adfa34d0b7d8259e6014224b92bd897c83226f79fe0fcfc4cdfaeb6d5b40064c64caed212e74b6e3f43917" },
                { "bs", "aa8017ce2528f4bb6789205d1e382664dba855e4bfa14a7f7da6ad33f5fc8ef75b9ab40c6ca73dd607e2d8a8401f16c5dd34e58c3cdfec8d24e80676afcd93ca" },
                { "ca", "dc08263e18595f86e46130c40585bdbd3621a1183e0d1c76c8e501a3f762a0e106e836a6dac1d432510a761dd4b8e25a4c9e703c5b4309dcb1384380a26a7d59" },
                { "cak", "6816e9c7c779e9c7750eeb1e4483aee5ef7805eb97fcd5fcb3c367511cc21b9b98eeab5f89937113e9330f5d7a22515fe136cb9fd5ca1093c103dcd4e08a06d7" },
                { "cs", "9cd2f182f469824fd1c284e9f6f10a283b928b3fb77ab1e1a489062591b1b50bf9e380697a8d9c582eb791768216d1675be014f959b65a2abe412404ffecd2fe" },
                { "cy", "1dfedc85bd4a98f7ce5725324bc901fe24e7a221b948b3d103865a50a29182e24cf87688374881771757b01407553b4f9d381c3718686c572ae23382c23e75db" },
                { "da", "840047c90bad501d6519ca37dcfc166c372b500d5d7696acd9f56df34315ff68962b91c1141c19d830eda33f7d3a2c4cf584f5d47950ed847a7957da20ebc243" },
                { "de", "c0117df8a41f5e9fc5b097cf22090b1b0f832dae843c2572a2b0698ddbe8d0267ef8f07eb356f2f34cce1f2ff58b534fa390a830a13ef6f4a13d9ec3db3603d5" },
                { "dsb", "397091740ca02932b085958e3b932d79c8d22822475a7a80d6666f0afeea799ae2440179e932ffb79eb927747b3574ed266dd6c39a76ff85146032c3b0505f92" },
                { "el", "130d8d3d2418ba549b37ea421a08b782b1d2061cca56a4e42ef93558ee6a5024903b71f1052c1c446ad6aa69f16908635be5b01c1cacb216b66f65459da3cb88" },
                { "en-CA", "5023b07f069d8dfc66f4b6e3cc0c8e7214260c295fc20072e51dc06e3b1484a46eed5ae5397e69a0df530dd01afe6ddf8ad40b9d0f43f647c0d0f741d93964fa" },
                { "en-GB", "f8d9f88a168ff22c228a7648d6400d535340a5ec24e758a0394037d93ce091c2fac989dd16c3c54a5695d2f2b42bd4a7ba1024c6fc258d613b460354f655b476" },
                { "en-US", "d95863ccf4b3e5d146cc1574551c86339a83708094c8e556862252965da479a08f92cb259ac437aaaee85e8f290bb7d20a9200d206a2535a0aff436dc88f9423" },
                { "eo", "3504e0a22f8c01d79e69b38f2b7d5125850f19906d64876cc808b7c0649705d559f3afa757969b63b839f79e78ffa16cb333e8181976105eb2a4c706a0d585fd" },
                { "es-AR", "57a84604fb9ba4331344ed42821035769b5bcc4ad97197afafaef8205afb36664ed50260ac7f82893646559edf1d7174e2e7db32d3f3626984ab226763c982db" },
                { "es-CL", "41a991883e2ffa642dce6392a4cc335720bb23c74e182b1664fedf39bea5b83bf99a6dfac16fb46f2a970973a76dfdc0a6f4f7ea715da2b78e4df4d46e611d54" },
                { "es-ES", "1c9eddf8a89318966ca8ade1a9668e0a6614c39d66071c2db52b5113443bb21631b9f206963061aae002ab7145a497ec067c6a796479bee9e54a2bfcf8bddb0d" },
                { "es-MX", "ad3b6fd6d0a243ec30bef62573dc9e44e031dd28c3e737ffbd56163a2c4680bc851f57f57fd3a899c07566922bb9ce2fc2d5613a06a931caf0051c8acfaa6bd7" },
                { "et", "3bf1222beef10ae3c67d7763cc90d90824b2b1ee8661d7d3602ba5eb952af3157ddcde82155bb2eec7cefd794abeb2809bfaea3f71a8ffc86863ab16baa80d41" },
                { "eu", "eb62ae157d9043ca457e18fda79942720d85d9a8f4db09efbb5f83a90a4bfbe6654774ddaf3559316ceef6660ca13f3019b7716b01f4e709cfefae5cceafb185" },
                { "fa", "3b425b4ff939d642d8e7fdf2aab2111178ab2d22608c32e1d79ba121be40a56d7a9525f653fa20e7d647bbf5db651158453abfc8bd35593f6de2215ae076787b" },
                { "ff", "afd615f21209c74a5f5708a6a17b5aac19e3e87d4869e4bd6f6cb44fed908860043a73907a17d5ff179cc0920123652ad3ed1de306a1cd32917cd597d101d41b" },
                { "fi", "5bc5b2a899cae25ff6b12cae44fffd1cb5df82dda82cc8c64a171912bee7ab30f5c75f3de01c9100473145e269c611810c7cee0dc233f667631345fe6419b860" },
                { "fr", "f96c0324a100b2b725b62d94ae593e6506ff52f5e3417077c7dd55164863c16e6db54fa771df8d5a3acb246e98be68463307f9e5ccd7618f7c1d0f09956c505d" },
                { "fy-NL", "d74b847df42861aefda1039f38b5d5af9be779e6be2f3efebf8249a0cb3aa29201c8cdbb55337cbc0cbd235afec3df1fa863b18ba6f9c1c4cb3dca69712fdde0" },
                { "ga-IE", "cc098406bc5ac99356bd7fb43b3efcf79e566bf9e88e65ad0c11dd2a6c561ee1915c1910b3674a6e092908754b1f70cc7b45b0acfc538cbfff4845c7549bf44c" },
                { "gd", "866e62f50e9a538888e1ee969549308a87d0e756d88d336537250d4556770b0d3c3a24e3b93bfa670498e6f238fe873c25992000e218bd64e0c774eed704929c" },
                { "gl", "5f51043e1fc7c07e1dd692516a48cbbf7c1130752bac9c4772501ecd2f09f8a9a0421bf7b349b2bc097b35441dbe30d3ef058000a893c66b67802e57d55d1344" },
                { "gn", "ea7d8ca079b32239b695745f197b7cabe0fe585615bbd2b1ff1be2dd7343b854ad79bbc35dda77afd5432059bbc5bbf5aa8645f27a91b90c4260f9b12b15ab9f" },
                { "gu-IN", "179205c102b5b7d178454b0a44cbd3c10c097e9eeec3338b756e7a63de1b36fe85bc0681d219b1932314932d58f41fdf48380bda79b6589155fe8aae1c3aec6a" },
                { "he", "8cce4106d4d1af9820b9a50ad8753f2e190299d4ffc9e8ac82d62dad2bd9fdffb5ab546b55c7298218e4f2a74fddf2bf2562431ef886d6e6bc96913659eb955e" },
                { "hi-IN", "18d4244f12d09bfd2174f0c4b8b7a15c1e76cc57da6bb814d3e9f77af4c1cf6abf419706b28f1365b061a0ff2c5f310b94bc5de814297172a9e327d3387db254" },
                { "hr", "ec28dc66b0e3d0e96764faccb20c84e6d94f5985c7768d6ef4cfebd86391d46038995326d563595284ca5e47138958937911012a01782d6d27f05f4f87765690" },
                { "hsb", "887fae2f699d3923c227d48067316ffb0fef8c044d75f7363143ab9805dac4e66598d12ff53f1a83a41480d63eb3b20d19cfbf4cc02296c38309d67f11d5c6fa" },
                { "hu", "7e5eb26df7841a4757fe8db9883e7f313311fbd1881591962ff9574b50434bbbe9032a302bb212307149cbdd6b84a1b7d87675d524b1d8dc2673b0950bc42985" },
                { "hy-AM", "10b20ab9a465111842b6013a3858e8ba5a6884df9815d52144050b2a6ff7f3d439cf9882ce4001c034396944abc9e02a1452972103b828fc3c3307a4b5ad9e16" },
                { "ia", "1b0b7601ec096e35c5412df62e33e2021ed75ce163a78735d68b19966245b54debc2671b812ef42138003681295f816250371ac6ff723abeaf38ef4b60a3adfd" },
                { "id", "3d37ed0696a0c436ed3d18854e4c96a6449544ca850d5e00422e57a0002d6c76f36eb439454a10a0665c270bedbbe9e6e5db49b596defd4af0a84bc327fdde0b" },
                { "is", "3eb5d10d9f107e256dda08847a152b58c277d4a7826ddb33c7af6dd5305d04732328bc8aa9cc57e13b754234e4eeca399ea9f47942eea7d2b2e00579e0824760" },
                { "it", "d2cfde0b79612d7b10981fe5d3dc99f852881752f3ae4b79fbda30aa4228dc37085c85429c081e63216e0f5823e28ca4b933aed2d941446768f5575b44d7a809" },
                { "ja", "76ab1a518101c2262566bfb3dc042f7bf1539ced85b4d0c64aa273fd7e593f5057716f3e8bee8248c2974964afc21aa5c3ecbba61203e84e4a87bf4ec7906cf1" },
                { "ka", "15fbb548d48deba877d49456911136bfcd493ecc9c4e59560e13aaa56ae07bd71ed5cd6b0d9ed9bcf15d5ae5ad6a1e35c9f56a544f83545e484eed4a99467ef1" },
                { "kab", "594a79c02f261af7594570c6c34fd15585e2165f715e39afd6843a1e90a8b4db917ccb70438370ac5caaefec16f9b596349646116d0dd5f589a9e86e79492cf4" },
                { "kk", "4b31a8d2c36dc9625980617624f2af32983339b131e97d2e4701f54dfef3e3b982d343430ce5e7a7fbe3ca66d04ed47bd4af3d53c909114af84a276308f6db4c" },
                { "km", "1ce433e92143b330d1ff108971e94a8aa32be530b8ecfd9e80c01fa1ab670c9edcf1c088f1bfcc3d5282fb93019fb3293306e24bc2bb00fcbbc6ba1781cc57af" },
                { "kn", "f2bfb16719b86833bb605d591fa0f21d02e822ea5e2b02e8304f91f6665051e9c863a5e8b042724814e0ba472d18f19a3c04b46114bbbc82d6a0ed28ddb3d254" },
                { "ko", "bc171ae229110ef29692cdfbba2d98a652a37a51f3ea3864747356eb520000149701107e4bca52a46968252951530fe7a67f9d35a9a92d9fa3cb6b023a50a17e" },
                { "lij", "54766bb1d50690158b269523c30efda1f9efdc17d07be4e520bdfa9f82942e7c0964274e996eefa7e5aa84f3a7c7af92d178da1f24ab08be8e0d3907c8f6c4f6" },
                { "lt", "9f0da5188986e861f94f98f8af5fc80f8031c74696a834807ea652caf26d1d373e68e371a978122be68046b3c99134655a3a92566a81d23f776c34c3b412affc" },
                { "lv", "36ee5095f15cb47ab8c06af41e352d7a9adcbb25659a816fa4bd7d844494c4d209eaea8643589fba486685b1aaaf37d33ae0eb8cd347c3fec9ccac55586dbea7" },
                { "mk", "865600f4861de1883e0a7799086af2cd37c948519d475dacdbe4be7805bf2784006c50b33aa0d78ebb99ccf8a0e26673084c65b54ff3a64a2b264fd8cbba6667" },
                { "mr", "a7c6d625d16b3452520a509937001b34f880ddee17f5712cbfb4c61b44b36c6e9737bebf6a80379195a84ea9aa2e5e80eaf18dcad2124950e16973a9427cee18" },
                { "ms", "d866507077a1c4f2d90022eb88918cc9a7fc33ff6b365b3ff73a3ccfd937cd5b2e3214605982324e49a914df552b15290e74c3652eef890852cd28bd13f05b7d" },
                { "my", "8bc75554201f7964a5c46471e89a31dcbb2f57b4e8c45f409bab2abca44c2bf60255820fb6a58e220e14d7539e10a010ec1a0120eb5bc698e31c4998b3568f85" },
                { "nb-NO", "473106e6fca6aed12678548e5c9741984d3deec97b413d9cbd5b35ca9be105bda3e7d427031386a6f49ffae00913c84aea3d3bf52c1fa38e5272e0fbf9e6838a" },
                { "ne-NP", "a56cb0db8e36ac17d623b785a24e5419943b80c4bdd4baa59e62980d6e276a0408db93d2b424e83d20147666fba4ad6ae7efc46125f1e2f502b6aebc5c93bc27" },
                { "nl", "f582bbe7a2d3e1788f0f4335d9f715e6ce25057d4d5e2c5ae9d2c68fde0588af5e983f885dc6cf53b25a49ae8d52a66cbc83b532805f862b97b7087caead0d60" },
                { "nn-NO", "e82dc9000bb48101f22005e75a693b26d8dfb54f721c72b59a992ea4ea53d3cf2db9adb7db701003aefbf2b3225479d2792315a5a5019fbb355b39a8dafdf985" },
                { "oc", "2c5e44a2b7cb0226376d3626990bb21a54170e0e276272a93b739009450b9b3cee4fea682aef1eb9a5cd844a794dfd96623bc452ef8f03cb6a40f385c495de10" },
                { "pa-IN", "249acbe9e205cce580ae5424ffb5133c43d17f341b52c02cb42468d0c425313c7297eabec542d28b3097d54016e93b74b072728ef1d4c6123b681b5415a9c436" },
                { "pl", "e6b4debdf2ac83c8270658e0f9d1bc4b3502b382b79dcd2b2645c20c31690c930bd29f5697ead4c4e681b8d5a1efb2040ab07ea62bb1a67a469a49b42725f46e" },
                { "pt-BR", "528a4ddf3fc14f8bc0f301806e0f4494a0a7c9c4fbb36aa7723b815bcae0a4e017dae4557f86fa91832de93f1785a8f3dca567d017a28341bd0b6a8135e22634" },
                { "pt-PT", "45f88b66fefb3d7ca5b0d71f129b10c745729f5c7a71b208179a4a81b97bbc35564c0328610c50b8db64f72b4e4f7259dee0331a0b3d5dedcfa98770ee4abb12" },
                { "rm", "e1a6c1afe96d275fc578d5f4292344c4ca49a03f51960b6b0b2e83458d5d574016f5f0c8f2029275340b9545e44404608064ae820f1e2d9dcc4e2e6684c97dae" },
                { "ro", "6e0c8342ebc8782dcf489371375286c43c75e15c24d89a31ca3e732b4254502ba4ebecd86f80ef4e1600817e9f4545c495f75e391c5a581105ab8b06f0ebf64f" },
                { "ru", "c522bfcf28941e4c8ef7c4658a0d89330244e741b44d0ac96b88eeefb8df883225b04c1af2fab4ed2efa3364323a2be1320670b66fe0086c3744b2662728b73b" },
                { "sco", "f72e9ce970bc7c857eeee7c66f8b202877520bcf6b5c6607d635c83a0f8dacd0fccbdffc9952bf2b31109144327a5b0c6eb297177c8c21a9b13bd37add5186d5" },
                { "si", "462e483e0758bb598714b092913620b53a4c8e045715024fbdb3cf6e12953bc08cfe17e9cc53be23efd95a9020ddcfb08199e656b533c735e5f9d5040cfd8b7d" },
                { "sk", "247b21489de84b0a63fc91eb49d8df3189764b84ec4175d3e612d069fa75c67f81425d297e587aee037911bdccf734d840da27e5a7bb99fb93beb926c3df5e62" },
                { "sl", "aaa151a5f1572c650a957932983afb3bcb0d305e9e485d441141587a9c271750edde75e39a136562a2e06da9ad724c1898e7e96a3f818a2fbaced1a324af8fcc" },
                { "son", "34ddc1c334d51780ddb5b355f6518215ac5633dc349318d683c89bb4221c67f25acabb66a5a30d11d31937c3440a595c8502795fae2080d606dce9e3aab21cd7" },
                { "sq", "fc9ffca7b3d69c9c96352d9832aeff6fb4888e15218e2611e9c1e6d727c266f3653ebcac4a91626743cf7e46984fa32941398d5e53fbc023e15ed8669f7ac845" },
                { "sr", "71965c211a6a6ec298ba88ed1f070f25d4e6cbac8371b609d3998e65245815c678305fbce1e0beef7ff0d3ba8d6b47ca7e9801d4e212d491fd91f66e510f1c57" },
                { "sv-SE", "53adf60dd847f184e0dc244a76caa18ee59b68a39f9833398df2cf413119b08b0b19425bf181f465b088c1cfbc4464cda32c032c20e9d1b3b53d81af947c38ed" },
                { "szl", "6203b10aa30aea6c4d4a292fb5502d759937b1a596f7d384b489a00fa5b1d1c26b74e12776b62b0a2f831be4449f4bb520a5a05279de95ab54e5e0b9da1854c6" },
                { "ta", "39a6312f82ffe114ccd27922845eadb8ec3feef2821b3f692534b92a54bd43ec1818d7e847e43d7151fbbf4d9b12258f3b98e862b696b99391a10de7bafc6a2a" },
                { "te", "c56d102a92e8d57af0acf6a9a42f3b5a5340d56893483335c1fee2b70e279c704d6f7bca70bb8a30f7d003810639d89ffdb0457afbf17709f1deb6d38c95a47e" },
                { "th", "2fbce770cb8c92503d80ca508079bdd6d36bb76bb58fa621d08f3afb0d982c3f40b62e502519a2f50fabdf9f5480bed3761d84bbcf65794af9aa6288ce172acb" },
                { "tl", "e7f1c32ff701fa17579aeb102e388624d66432a3ffbb8d32f1d45df4761fdc6577b1999bfa1bb894f5f40b6da6782b0ff98833598484126961467dcf8846faef" },
                { "tr", "090109a7d18c931a8910da0b797748088532fdd4e6779949e4d1ba6e4d211024f0cb0d0030a9d6a3ef9f5ea9556fe8ed058bcf738ad1a873592d8d6515ee4f54" },
                { "trs", "0efd905f5849308ccaffbba36a58ab31c8e13c74119e35ade1748eb469e887f2fcde33609b66aeb8615e3961d3426c42fb1e2bd7aba3b5eb93dde4c187058bf3" },
                { "uk", "82ce859db92fabcd89760d4cb4a73a213b474c116bc97726db59111f5845d1ca49b5da46f4f0b95277a619eb38a417517e328c69b497098b7b412673b554f71d" },
                { "ur", "e1d5746b77547bbb16544015d2d860ffc5390923a774b1b46d5c6e7b5d6f597fcf3d0ab00d5c512792f505cec935463143be607367e6820b992ead4677d613bd" },
                { "uz", "478969bfc4e962fc8427b3273c2fce0aa061867453803b017eaa859559e972dec14977af9792bc90a3b54060dac8cb052c13eb0dbedd3bf9d1a4afeef5987439" },
                { "vi", "7b13ab3ae33bf7968dc465beea57b93e53a95a93df1ad51e707199bffa4b1404ad745cbc8b4feabaead2036778ffa9381de4868980379507000ccdc157f49ba3" },
                { "xh", "71fcc4ce1599697e9213dcfcad4d346eae9a5a116cfc7574eb1ed405227c9392642d1a8bd34b46edfa6906d028efd091516ac36bb2df1de45d85d34e5b11e437" },
                { "zh-CN", "859ae3570c26f5044f0e9f5e048839814dc61e2fcf4e876b9208e9804dd75ce2f1823c9f15088d47acb7fca0b937948fe595df65c947a8992161ed9b8902bbe9" },
                { "zh-TW", "b4ae63d8aa7a02d603f623d56ac305d6bfc974e03e9030f3488cb3cf538eca0422c43d0cc1ecfebe99cb6153d81a932602df1c4290c0ff09dacd33f815b43789" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
                        if (newerVersion == currentVersion)
                        {
                            checksumsText = sha512SumsContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Exception occurred while checking for newer"
                            + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                        return null;
                    }
                    client.Dispose();
                } // using
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
