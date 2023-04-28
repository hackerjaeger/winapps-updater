﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "113.0b9";

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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b9/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "841074ae3f803e629f1481d881480ff4c4a4228891576829c5428ea686b54192c771ee1c2bc2dff7fd96cc6e668702d407e9a894bc42135797bf04aa3c69761e" },
                { "af", "bba64517b9f0187d870b5628522445c6a0d3b2d2a4e0eb132ed04745a6660e9a0b258765f730bcd32847088686c2e251abee534af37325caea94d317507481a2" },
                { "an", "9981dd3f9ac938b9f8ca407b54b5f1abf65c61563eaeb8d4ba325b04312cfa862bd542b1b9189609b03fd4b180ad7a2e3b7f9387e1d0cfdd2164b9c80ec07cff" },
                { "ar", "829b16e0d1b9e5b18f4296354b2ee8822310f982cf5ab2a77c0b69721f181a25e80fe0aa45fce399986329b4b0325321a2628638664ac32c730c13e9ddbce2be" },
                { "ast", "7f3d83a18eb6891a0b1e2f6be9880f88326095bc7f0aef1100e84336a3ad7f9d9622918103c805407de1a833f9a35ea939e7afeb44318d1ba343ea3f2f22a462" },
                { "az", "5bdae620ac0af0b0abe144dbdc33c1ed766a1161955fd3d93ce71284d3076652226ad89032a82e09defe00929b0f3349c4771d1b80845bd0523cb572d64eee1e" },
                { "be", "d37d14e3c892e5ba38ead42de35f0ac6942d9d587784a414b54da895dd98b68f46da7ab47151ec83f9143e1e84758e28a1f0f105c519a1414dd15554c6f44f30" },
                { "bg", "b985743a36abd198b736ecb0bda0bf1975664b8a4a22d20b6efd0e4062811baf05b55248a17917a2295a1742b7ee63fe8e76c38445c3a57e26c0a27679356180" },
                { "bn", "853736d7ae0f47df8f976a1c875919fd30f0bf500c315673920510e0fc1d552d444bbfcd539b96b2c0c789ad6f09f6365db58db45d5d74a9cf810ef06f18d497" },
                { "br", "e621365a5ebe5b20e4f3af045985c3beef06d9e5a6790b3b9791280f8fd46311024a9a56926bc3240563aedd66e8e32effb7f6df4966515c64bd179ba3222579" },
                { "bs", "c2dd0b4a57393a19076bae9921db1de63945c18073196e47448f19e6e2d1512cfe71d18c54d9e77877a2e297d6baae68b4a7d6073b23595843a1e212cd43f841" },
                { "ca", "6fd4fb41ae848663fdd2e7ffc639e20fbe284c0cebc0150a5d80ce3a2772595709ac59ce363345112c79fc2295551fa8a3b2eda46a51e67f40aa782971e66f3d" },
                { "cak", "2a7ebb00992d892fa553aff09060c70bf2085eae28b43d1cbee3c2545f4db19ac7fdba96db9a826c050f7b5d80a5342530886f087430995522bbab68a16d8b00" },
                { "cs", "84672f76504f07849029289da3082c9ccdb7a1a1bd48b695b5ed36cdbfedebf365d8f67c2fc78a0baab0f58612ca4aeabaec4c495caa4782834f51ec5991f126" },
                { "cy", "bdb535d3e1a9c40446980a11ee5fc6e31e5cc159ff7736ffdf5d3b673dae22c56426490c779709490fd1dcb71f62eff550c08b76376e7143081f69b8126ce120" },
                { "da", "087667a4812c24f50aea81cfad5ea61e9792724bd54efcda5c34233cc6109bc5036c1ef594c6536358757a3c39ad67c19905e3b17578826727617802f7ce80de" },
                { "de", "3f2a9880f08aebf4eb071932e46b5cb4dc7682f12ba06c46cc77663b0f18e706c05946b47cb296e9a3cef1ad83a51d05b68132803e2dba283a723dfd38822079" },
                { "dsb", "0890ee4ff5bbb650d7eb038c5e4fd94733301d7694dafe23abee1ac5c70c4438bd0a7972bd032b514f5f07cb6911b97166354e1175234925ec84aa55fdb4bd7a" },
                { "el", "140c3ce159bce86e96753a8a74f72b332c3a235618d86adad8e1a2e72109356e18bcbcd61fcb16cd0729b0a6b4eb13903ec553ad289653e0f2f1ac819b50d074" },
                { "en-CA", "b73199a7b1d4ac3606b213b1f1bf00602d7b998650ff50b3b53cef762f8dcdf0d22c0fafeec295ba78c051a577c3aeee2f49101775ba91109f93b796f067689b" },
                { "en-GB", "bf80da8b1547b03c8d81e23996760a5f832ae0ac92e3ae9f5bfcabb4bad6046f7b82b65c8f83e57f4e27729fc428e8c354fd59494ff6abc29f8baff808a436fe" },
                { "en-US", "13f0500f9c338b076058f7633700ff656288c2c4cfe90f657a66885d4e192940b3ef97c5ebf6c519a9a8d61ecbc963a9260d6f36d56dba7e3f2349ba8f2a427d" },
                { "eo", "4e2e7b66675a8d82aefd6fbc44f631d5ba32341665dffdbb35220e93492308a254eda927f91979d1e87fefed5ccb233ed9573bfb69412152ab2c09626d4f6f17" },
                { "es-AR", "6921cd6220bb12d851897a4d63e96f83d3e1e733628ac8f13edc097e68458f8fca65a0936cb9bde3b4ce8a9ef84a0233eb700de23dcbefa6cb314a3ee88ce1e6" },
                { "es-CL", "add1a1bad7a7d2bd116b07cf43eec0a685161102151c11765a5c85793b103c51c86f0d95663a937c5310a62bffdc41e1ae4304d6917284f440c17749a8a04e40" },
                { "es-ES", "828636ed9f4ac1e0d496d9a1c2568285a40bc59ab45cbcf907f6a716c835a09ee8fc5b7a95b42cdd83f960c08b20c3dffd56de356a69ff7ccd589e254b548d4a" },
                { "es-MX", "f0e9acaf6ed41714cd072b48c71490198b52745aacfe1ce974f31eab4a72232586f66d770323caa0a0fa91b87ad8bcc81e182f835eda35ce2c2b5097a56a8399" },
                { "et", "1daf7ee7f222af0f84b4e6600043018be699ee8403c5d68c522e9f66ced7a1e7a84d6f5d430e1e49eb0782cc730fc8735712e5c1380a4c9dbca479970c098a27" },
                { "eu", "09446a4b029a5319a156a66294f6c9353b8e4ddd7c35a1114f841a520480c5b0a4b3a82c9cde3f8c3343639200d148cf831f7b50020c60f04d0b6c7939eca7b8" },
                { "fa", "50687a53cbec2521e5d3aeed2147d471783b8a1232dbb945ef69105beac45477d2a21647856991b7309cca407ac81ea661d19a5db6150fd3a5cbe719be4b2398" },
                { "ff", "01b554e017e0b0b6e4da9d99078d0023204d26b530072bd2efb372e69edd963c63bd54864e6e2b2dabecf216cd109bac47995a90a3c02df2777cdf0e0a586582" },
                { "fi", "1d358fa9422b72784080cff1b33e4df84a29ff7f116c83e4db57f55f4f2c797a52e274ddecf90b89c909ea3036f885333d902965300a598e2588d0fa97193d20" },
                { "fr", "375590a0268edb11305058b8ca164bc96747e999f6aa746e78db1d82a1884c6884735831b40b4348d62ac80df3d6881887712530933037ac6a39979458ad8f8c" },
                { "fur", "08fef0df6cc7aa718e9d2adeeef08843d5de779bdf26c6b6982256bcffa5d8270a51e75f64c64884db2b0964de74eec48b9489551778fa93e1cb2c597ce2fc55" },
                { "fy-NL", "a7497cd88d666b21a66cc72598899940c8f539192d1fb2dfa0d982513ba1cfefbf895a30d17d86e35f21c677fa28c300f1a2efd14ab95f6bd8da8629409edd92" },
                { "ga-IE", "db4f03305005593032827e034f6f2e3495145265f4d43852360afe484e0d142d9873d2324c3a4b44944833d2bff91d317eb94cc05f4937f925a98c4f6e69850e" },
                { "gd", "e6a3ebf948d21e6b2e22a8751288bfbf4b5d911cd41afebdd664df5896c4c7b4e34fcab44f1010dbea699ba3897d0cfde3d6e1e10a8ad8ed5510ef0076a9f79d" },
                { "gl", "97bf9ab89ccbeb7bbdeb7d7f4644260cb22fe032403aa2936d29f97fd5237f13040d56a0bbf414590688ba132408b7c11ca75379d3df16ee6fdb55ea5a676fec" },
                { "gn", "bda80c3c8c459a1822b0846a22e315330971ea2b69fcf82b34a1cdebe3563131a9d3457e5ac3650f11ff426d9ec25784f17a45118711c372a5ae31cb954266e7" },
                { "gu-IN", "8bd76ea08003c9cc4cd6cd36221f44b9f2a8960359da462faec91981594cc91fd8931de5c035734ee77e7b06bc05528e1a5781cae93eb7d743bc0a67fafeea87" },
                { "he", "05740dd0957cda13de913966487be7595d382a60ec3c1366e92277eccec4ceb31ef382b241d3f5b009194165dd7d7a5b0cd923408c20534b1f1903926e05ea2c" },
                { "hi-IN", "e1439b97f75433d6b69bf7ae7238104d2abb75b894995a0c5dd188cfff40b512bb7e0708472450e7f4534018f192d94875cec09a05063451d14d7f594d020524" },
                { "hr", "ff90fd2589435cff514c1fae305a8cadc595990b25821fd84b6038aa1423b401e8ccb8f91967596aadecf07284f076a1d6ae3ee0a295fb7318a584f77517565a" },
                { "hsb", "ba7c0fc3ee787bf973690de905812bd14b3feb8988ab07c70b0d6d08c5d4cc654d680ac04ca12a2690352f35fcde60134c5d6b27dea82b76539eb9c60f438622" },
                { "hu", "614e20338de2fd04067921d84f27a9291e9231a250ceee2829ae29d167f811d62a30098f1a270a93de24edf48b8de299052789278a58046933caadcd467f1a0e" },
                { "hy-AM", "b494f02ae672625090fb84403b9e1f4302537a89e9a496bc0d42c28a8b936aa59fe17aac3e6347bd1d0028dd4e75ad0e7e875a14fedf009cb9ae836ddd6b84e3" },
                { "ia", "fd78cfa1228e1e88977253c28cae92f2af3e1671edc77ccdb220a311f786eaca7fc988265b425a83b649438658f78cedbce1b92f770ca81a86a6adef19a4c1da" },
                { "id", "9acac0b310c5f3c02e46b5f525fb08fe1926c1a41bf3bece740398b8b11e46703f87158496209c464e7cb5a109d9906df68cd0f4660397f5bfd94acf7305e2e9" },
                { "is", "d735a2e4eb3ba6cb61077c6735ca2d03bed6c7eecb90b8fd57b404570b8ef9680dcd8c20d499bbed14f06f3df67fa5a4e8b8e510200bea226f2309a86a9d3a1b" },
                { "it", "7329c311887cc9adf21d298ee65bc60559c644c8c2b37d949dc58f0bff85c6bdfecae122c052d2a322070cf3a8d113f865b29244f04773e4c3b2ed7446194be4" },
                { "ja", "5ad76df588765201764efe6ec191ff9cd4212f93f5212a4169f929d3eb09f3bf5694a827511758bab6eb57a78e4bb3cba7de883578178af74f48b9ff997f79ad" },
                { "ka", "faa5fd04a09fb74c8285ff8be367c365b3ec42fdff652bfc8fb1b74ce6e35aa833c96494e26296d16ab6e2f1ce28065606186d3acd750968e84cb6d833f9ee0b" },
                { "kab", "c9e49fa4bc5bd735f4ac090717f4f19f0965d7594ea01d0745cd62c864853767b4ee2ed72a4401942c0b1f8c3ba41a585022ed7b48ff436beced52718f87325e" },
                { "kk", "0b03650811fb3d6a9cbd691678d784621edf758a11e8e08c84a5c042a145861b36999b3b1f6175e46f0889d27524812750da2a7db19968b2eb2ba0e8cc1fdee9" },
                { "km", "f28ed9f851dfb938054fbae792806601fc4e82b8f602618217fcc4545ad31158d2d591610d740d9d3c6d825f3f80406e1c783bc24e00815a5d5a1fd985516312" },
                { "kn", "ec9ed8493341607171df745904648a17505690bb4f04733fb5e4b95ef52a58ae9ec8dc442f29cf6c3ee5a4ac14168b012c2675eb9425af3d2917690d5a6c5b31" },
                { "ko", "d4db4cf569b7cc93c7363cded9b0d86fc5faf1a3e7d7b4197a85f782aed2aeaf5cdd3451698b81c2119fe160500254b238b3f6b052564fbbd3333f94f87b5b2c" },
                { "lij", "3f8bace8e8cc4ab10463a6e10f49ea6eb00bdb065476f320bb74e87b2a59a5f3f07f1db0c4e17438eb83b4a0f80f4920a5622e83c6056d9a942fa4e17c2cca9d" },
                { "lt", "bde08af2819c638b6326eb32458c502c5ff16ad45f24f8b5d9f014410a26e47d4bd83e8127f68e504a03d7d7fafc1c4197ee788e1a7e1ab4b878aee77b408941" },
                { "lv", "29cf1dbbd0d715ac32001283e39b876542b8309cda2f44f2266a003e0cdcb32d9712fd40becd76a3fc51bc6f567d81a06d64828e9b53b195bb3c0ca217b7c7ea" },
                { "mk", "126ebd40e1f64c4231d42b7dadf55ee1732a0a381b1b36edd02bda417e6d8e84b53c0bb1b2b0bf238d218f95c5df017f355f1d308834e39241a624186532a1ea" },
                { "mr", "4abb8001966b9281a219052a0c887f73eec32922f9b0ee596a5715898e4eede782445f69662c00bf1b753ac8e0aaa4d8b8e14ab2be56bec38d47c081f357f5af" },
                { "ms", "c181f9fa79cf9faaed6db4c4bf4327f7e6cd553cbc041671d34c91547579f2c919773ffb26b3428c4d9515d398c53be378bf9b8d7802368bb0db718ef84e5d88" },
                { "my", "7c1e831a1ff842976db3273b8e927806110a81499c3df111755aab9c952b5d5aad07e25b2ef41a476c779166da0a786a0aec947d15e9141e29a5cc862f33655b" },
                { "nb-NO", "bd52e8183470b152bec3f73eda79c555e6150b4502f2ef3df087af2f5e761a5275c19d84e5412eea6d54bc02c8c864d0b55306413ea00e6e1b50f99824f2787b" },
                { "ne-NP", "0ea887e4f2ebf252adcd3032c9a4a516f11755acee58fcfb7db173e4396e7d50f5e15e8bf4c1ddf7092792b5eab1db8f7d94e4b1522418740aa65486b6c38918" },
                { "nl", "7334c89d242803300176cc0b5d4df4e879057fa2447ae6b7300c05cd71cd2ccf43f44b1101430618f27432cc0d8d57dc0c45e167959f107ac2885789c7ab07a1" },
                { "nn-NO", "37682aebb8174a74911a9ce0ca732d2d69a4fa4bcc9f55c7561bd849797355eb4879e573324d3fa30ad001fb34024f3b5492f866424b74d9403f05d2cb5ca612" },
                { "oc", "eab14b262a4e709a34f0f0cb213e0e12ebf5d0b4bbdfc76fdc590130a6c658775e0698bc6b285aec00f8b54b96ee1a8daa3ccf2927c24e955cdc33e706d4b0bf" },
                { "pa-IN", "a2b0e5773f01e9e9b248627b97a4e861edd64f2fec7312bb3b7ceba83584021fec843b627368d4332de9dc8657644be5c729ed2bbc26082d69f7491e19755845" },
                { "pl", "17371246fb3a2573ad06a9f070d7c8d9442423d5fe4c73e77a8fde2d02ab862d0a048370a2b1ecd2c361bd78652159b66bd8d64a6e352004828dbb51b80f7bd5" },
                { "pt-BR", "be202685275eb1f3dd98c2c5e9f1b87e1a007185298c82957640179a8822966726252964846f445dda47f236697aa7fd5c4945faf4c1f7c584dd7481b933fd14" },
                { "pt-PT", "ea343d66b89bf38317f260c0bd9d563ddf132b0604a03d34eb30c37e69e4102d261a9e4ab6b00124ecd471004a7f39cafad6d267c6fb1bfd3f8aaf5b6cd1b17c" },
                { "rm", "039cf28e6f70a8e532cbbee67de5dda466f64b5971737fddb36a15d9082f0f8a7e695d816bfa0d00316851069a535bfee772bbafec4bab094c2f5d96adcd7fa9" },
                { "ro", "e68256215265b868b8d2677cb515f4cbe5abec4f2baf26692798aba2a8d56d4ca42102b1f15c45bde54c9d4e8ce5b99a1d7d5b614e02ae6fb5018045a7673c59" },
                { "ru", "ed9be5425a7240f87ea7198b085e5123fb01a5af5851adddac29b7e5a7b70e207422f326af32ac33d5f94e5be60c0c869c3aaf3d67e04b0530fa5f05c8ed2773" },
                { "sc", "8f67a47913949f7b93d72acab02cf95e9c2eb9c643ba2ca6c414c93e66d082559c154144ed430142175fc56231df8a91296921c2d537c4c605beb1ab01f1c529" },
                { "sco", "20741088a581459a1ea902f044c3a1d1db6cabdb8582183b89a336a874393e864d3d77aa076ef1aa4902cfaeb8af293fc025c51b3f6ec9d041394c0fbcaa3940" },
                { "si", "19d59e696451cc4ea99f498909e077b8e89136640e80d8d1ba9426ec77155bb9e2243e350fff78e22b3d14d75e743a1b8801471aa651b8a021134fd06b11d06a" },
                { "sk", "5779fbbaab93a71d76fe520349899ec64e586b686224cf88f7067893a5cc367868c16ee4630405d4e0c1ed76c26856351d94d877028b182dc6c4a10d9d554010" },
                { "sl", "65efffb6b750b23b5226bbc321da2a6a88981cec2b63d3f99875978ea109ab2331554377b7f353eda522c3126a721c082765d534544c6e8ed0c0257b2dc7d1e6" },
                { "son", "9576d018c1ed436e7ae3324382b1d73cfe1461420755b8965dbfb44de5522eda596ebf1eeec6f5be94f1f8a449d262893c499467f141bdde1171d9d6b396d60d" },
                { "sq", "17994aa48c7fd222f64b4f62b71685b75a148a94d9302a5b6f9121e24bb4ee63f5ae248df5989132ed37e11243d99527df10b4b28e307d6328e44305abe5c171" },
                { "sr", "191ebc72628fbb61302cd92c5815a2c88c698ce4dca221730b241096f9fcba925bd6f1bcb11466bebd114d1e631eede42ebd9fce4407b2a8c78cea25b4632b6e" },
                { "sv-SE", "1b2fc5f73d1dac0156b088b63d280ce92b2c7b30f0f61105273131cb2b42290d8c398b4dbab1c491210c02a26193860b16067f5542b238de7ecb897dfec8cea8" },
                { "szl", "216c0be47dcb037f18e774c4deb60fa85090aee9a0d9e96349ead6696c7747bc4c31ac7baf8036920613c98b4acd3d4d69b95bafdfd6b95e67f15dcf7598aa41" },
                { "ta", "0bcdd1771d7f7d7e0ceaeadaaf7c26e09c52d0242ee7283950c3f5d43823ba85cb45d1a59852772a8876fc349616de1ccb9bcb1100559ee5b6c71787217d0571" },
                { "te", "8cd227e6fd671baa214b3ba51438901a3bdc198ad914a36d4e699c9ad6e9464d486e14982e71028cccdcd27122545f1eaf63457914af5a78d591257ca4e3fac0" },
                { "tg", "e5fe735c06d9602ff8666abd8eaabbd750396c3ec771a2e6a153254ebde3578f859fb3c912acfd5e03f9ce426e9cdb9aba990f4167dedff34bed70ad96fb40ad" },
                { "th", "10a080b10a4b4579ca435992a80bf52bab628db32ed8c39863a127be48d55ff4c70b9d5c4bbb63bb1e503bd0ac8d2be627e2f11ed363e791a44b867fc1c2aad9" },
                { "tl", "7d333d1557dd1ccea39e855b9d8bdbc104cee044cb979e04cd1571bf5e6db7ec9d317222db2f498b3310251f161914ab990a12f37581b62305157cc7647ccc59" },
                { "tr", "b0104f85d2c10674b4c0291a45b00b75d7d4684ce02273c6a1722661cf572caf70ddec4c9dde76355fd5ab07ae051de6a1b309c5ce2d4485f00feffd9110abc9" },
                { "trs", "557e9c609634deddec6e9e6337818e8a23799eb2977cffc8f33fa4e1848d9ca90a4f6d1123253f0a4f97673bb923c6428330f1ff13ffbc6882c374daa339692c" },
                { "uk", "3fc43a5f2658092afbdd1557fcbfd0909dc1021b66974858fa63d3591e0cf57b11444ee6210df09e9c487e843beae856a0dff8e5cf9f09dbf1989d9b9e937881" },
                { "ur", "cd2c87550e83255dc8c9e3a85b2dee5b487dcf75b865a7422d8292a52a8898e07b57f3858c102b345665980024efa7e2a7e97300c7de9c8e805901e3f67feaec" },
                { "uz", "0309fdef63ffe237debafd1ad3b1de98191dc86a82531c7c55bbca8513f760c0153ac4bb88b30f4459bd63302083fb18b5056c9a23519644723eeaaa8fea84d1" },
                { "vi", "fd5bdb70434753e0b4d3b4a642366b586c3c65789b7be7e7453d64b4f64b1452b9627396d92603975676100d9c3021bd36601dd66800fc4498a5f50c52fca3f1" },
                { "xh", "1ce1cc20c97e345dbddfee9672c49d240cd497971b32d53420b077af5cb8e5e60ab656179543d07c9603651e13e7a1c0ecac32259e3a920d841ba6e20ada1f44" },
                { "zh-CN", "7a457d9f1e5409267c86b36799b300fbdd2bccbac95dd8c9386e48f0272b52340d4b820ddc2bf7fbb0fb73640787ba1917a0566d7327535e156a1f2b94ca0370" },
                { "zh-TW", "df10d6ee93ca14ce13cb51d365a0fc0675a705db8f5f1a40fe1292347cee102ca45fdeb0a13517e8d95da88f3e35441d5ea5ed95d8a7472b8a51dc1d78130eaa" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b9/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "7eac58fbbe67f99055186fb7d63fd00aaf4cd99835df7adab7661c12375a0c52bd4b5cc939dbdd206c3dd4991c4ffc392d76827652a27f38906f90b6122e67f3" },
                { "af", "d361617d58020efe3522d46551043427df3eb858996ea5768cc14a86db52032edade66d8d9a98b08d0396acab38b52fd3ed5b0259aad4fa4a1829df04e1e3b0e" },
                { "an", "ca5ebdee5af13abb5d212cadc52ca6c2f319c36fa49034d0a578418fa867fe8b89ab9337a845a12896914215e63e086b2c19066b524e334e606bd4f3a7216c6d" },
                { "ar", "fd5064056fc6e745e5fc55e09f534e70fd23ae60d9a0f963cb51aad6352306a2628657b42d96de9ef7b062190979318508a6d7388de8b25c2601e966620fd1e2" },
                { "ast", "40dbfacf53be67f671b7a78457c28bc6ffd39dd877f6d2cf47e36a40fc7412d922b0e1fe1fa794cb504d5415604620e103ebfa4748ec7db644f2de1af77460aa" },
                { "az", "0f36b2c685d8e22630989ba3b447d9b79c922671ab1206e3d514682d577b2e687b561b6c85021c1b8a23448f66f35fd206627a25fb8670b358147c14347a9580" },
                { "be", "6311d1d2929d1474b4d8448a7558c6bb3853713e0654eb3f0aa7a57e3262731ed751a615bb0194dd385a6c11c567605e87b846ac2417a68868dca91b3b9ece94" },
                { "bg", "d20dbfd471118a785d263d092c048d1c891455afcb44695e9f5c57a92351acdb72cc7b3618048459be15f3ebbd9d9990c1334826b0bbfbc119ae93043a835000" },
                { "bn", "0229a0f081563f5da1af6fd34702fcdd8ed3791ef15ff1e822e9a9dc9869b89ae1e07ff46f6bd40fb264ef5bec5395bd12cb0b3449454b158bffc24867a204a6" },
                { "br", "54af0bfdd0d204e77ff19b77e1c3f47a6801fbbda2da9f9e50a393e0b9e861f4d52cff96553532bef7766afb9fbd1d692253f5b0c52c3453ae425379fc7d30d5" },
                { "bs", "1da5605ce7597ad50e8fe76a8fdb5b1cc203801e0f016e5853caa273782ab58920d2162b294628922e9fc1c2fcf4b2f77dccf0c7d3b547adab26ed1564d4f3cd" },
                { "ca", "070b7a915889a0bb150469019991b2b9f308b2cff40a64fef24cd594a95fa95987091bf1cbf155b1d6bbafbcc1d19eb1ddc748c8b0e4e715a96e4f236667cdd5" },
                { "cak", "be90fe8f018ab5f8803b3e030c34584cff176db76358321ea1915215cb11067777e8ae1ccdde97c76a9aeada9b199337b6691c9738601ad619ba90c368025315" },
                { "cs", "f4fcde3c67abd89dd69a6911f121c3e0f0134b177c6d4e1fe0dffd965182a1559a04c054d1f61bf5dc708869f69553f0c7059c9d6735063df66183d44453ad63" },
                { "cy", "12b27cd4c04a3ea7b5c72e84eab184344f9d0df4babb251a4a939f50aae3a15a0d6890d14ec2a5eee510250f856a7f26c4de7ed8234d628b3007816c00c5ec8f" },
                { "da", "f5f92be8de40da2002307cda3b902b234068192df561a017605c1e613f9606699c82d2d5f7eba4b6c2b8b4a3328e7a992ee6670206626779e204317fc989ec5d" },
                { "de", "925ba466bf3ea748bfe5b146b5e11065ba0396708f2542fa38dcde0112ec8fd872f7ac47a3a474b5347ca893328fbb7496d6e7be9a1a267245bbef2efd25d470" },
                { "dsb", "025525aa44e69acc966c88c93c44ff6a8e7a2270872561a2aca3d901c0c029825900e6ca65b83a29e859eba20625b551ca56fcd58b92db5d090b3682e616833f" },
                { "el", "d4ce14ece0c48ad4e804e9946ba3153c18f6e1d6770222ff44fb7b0205868554fad42120211b99b6a7d96c3d6cc3dfc94c3d2e707640d7153a4156aa8e9a1e62" },
                { "en-CA", "6a2821946f882ba5b2a60dfccad53909b1eedd6f6767efa6904c66952309c77459ddd136564a75827d12d9952197b25d54a58d2b33b63656a71ddadd94711737" },
                { "en-GB", "8f38d82129301aab46b8a939af5fd39d786af481e8bb4586d9a4a77ed08ddbfa0eb0bf5bea2a0666112b0e9ef04a4c839b2e022e2820f67cba4b0980f9a29bca" },
                { "en-US", "c25f161b460d5a2b14e4efe9036eaf843f1af3af07871acfc667247ecddb98a673e06a1ee387ce539b6a9a3af58cf32fdc49558f4ef6fcff66d21deca6ead8ba" },
                { "eo", "be8eef41a45730f6ea03f5358183117aec78b409103fb36f9e740c0e8e4d95802b53ec2fd7379a6ddfbf9fad4ded3ea8ba5e0d6bf1f070a61e3240a1cf132df8" },
                { "es-AR", "647bb8a9162a6c2477d3c86d6c38a560510f6c55b435867922788ed916ffcb6ab1e24b228f4db163bba43d1e6ca229d3e85e30ce7ccfe6bf2e578327a2b007f6" },
                { "es-CL", "273a712d800ea15082de02ec9e42cc7751b411f0c22f7843171ec8f4be04d5dc4b233e28942b7e0ec4e8115366f8fe4ab4e7db11bd211c1758b2b472527bed2c" },
                { "es-ES", "a89000f4da92ca7f5691c174d485fc10f7e85e912a2b1b237b4a5940790d01ea681f251083f03da7ab9cd8f6da61fa3c308e0bb78967e376138f29d63a5dbfc2" },
                { "es-MX", "a788e59e2de88794fd53093f3110f6fab5f2940c0274a13f44a4b8952b51c978b698e34f58b2fe7a721c36e3b6c001cfc58af635804d1b0e4d7dc3c5ee6c2e54" },
                { "et", "eb06bf44cca41793cf915cc4347f780b27324074c8517c481769a45b62c59a5144383bb3c2941bb6a15cc07fbcce6d863256bc151605c39db89f83e1c34019f2" },
                { "eu", "4777e04cfcc99fa770fcdc7ff920aa8e73d3c397ef25216a01a8cc11e716b138bc670c0c42e8726775d8916dcb5a3d4c59973a98f1c659eb2b79d33fc07d7d99" },
                { "fa", "88bbcf6c20eaad7b970725fecb54c0d22865f6459635116507e4c82887989175841b315d3402d7882ccdae8e516f51fb72ff8a2161a3c803cddc59bcb1c2bef9" },
                { "ff", "bc36c15d19610bd5ceddfb839e503991d028be26abf30f1f0c49fc7b9349efad423213744828493947bd3aa13140cdf1f2598437007bd12f004072cea000f9d0" },
                { "fi", "682808c0edd968e2577d79e2b1ee620fe928efebb72ec231fe1b260765a341dcb00c5f9ac28935a1428f5098bf16ffe6a203511cd53d74dca1672bf0c910ef25" },
                { "fr", "45af611d09fa5897a5faaf7890d958d44db37dfa05a4028d3a3d1b32077372589df70dd071bb2a00b14d3848b1f610607318ee61819633a9c711a81f0268c29b" },
                { "fur", "9a37967a7176ec7df633f84771688b986c51d6b5d1ec364671f363f1c74e78f0c7eba0b823c5b026c419a112fa18eec9822d62efcb2b6093b909993625d3b85c" },
                { "fy-NL", "8108c2445d10cabf5910a21ccc7a47b5c34ce921dc978d3bfa0d5565670ef2f5e1daa7fb02e7118b0cad072e4b4d75764b608df0944be544fe6b15f38b10b187" },
                { "ga-IE", "e66fac1b27296e4e466a4d1b246d830c84829e114beafbfbad0f936b75e874bda0daa327446f8a6e237c23f8c338356383854a4f329367c55c6a58a37172489e" },
                { "gd", "e89b6ee7b4ad8c2d6c7c84ac0c148ce916cc5e30dc2bc93e80d2e7a7b29e2b5a135ce7847d57a963ce38303e8c05e3d57407989f2206794ab6ad2c323f4e8252" },
                { "gl", "bebb54043a57420d749cb03ed297069db3c1b72a12857c58015e6ceffb5a987aac88399f2fcf5babf6a2da7274ce6ff5c78d583b0703cb8e50ccd5af2292a5a9" },
                { "gn", "1e62f3f9fad39f60eae14417bbfdf306781c8edb89083d8745a62e4c4c5f48d1efea3b5034b74686d2b4bd67c20c9d1cd01b9fa26a736f0e0dded8c04a5a99cc" },
                { "gu-IN", "9c4c9f6ba710715218925858e3b577731d55907bcf708b2259240e049fac21f145132307d62f79aae2c74cdb2514d938afd2d072c4d38bc00d84af508f4a7061" },
                { "he", "cf3cc4da737d87d08e4569e2ee78ef4e210b7bf7d79a0bf482644c94d4ddbbddca8a2ff6ec9d4c526a946b88fc08de367e233637f050fed82be07c6668f24454" },
                { "hi-IN", "fababde726a81094bd3af92cda45e2e75cc1e5f119dc87baa3db3f8211ad31ca3d93a7ffe77f86a5dcbc4d3112cb578855b2d26654d77ac972430101e8147267" },
                { "hr", "0329e6df781bde6695696b559aae7b429be4168471b05f46851ef68576b23973f58ddd8010ba43241113846ffe20e0ee0fc16d31dbbb58e61b71df2d35758c40" },
                { "hsb", "5130c6466a737202512e3eedbedec2ba9f439d695dc32ada31462005532f8303a0cae6cd4a5fe3481217259040d82adbe2c1754f673d10451ba3689ea9d742f8" },
                { "hu", "eef5497a85dc99d59fa8bee516c1d75fef27225d4eac8c3fe359ae45e5907b4491cc9458447f3af5f90b1771a566cccd79234becd9a392118c31fa2c6f8acb88" },
                { "hy-AM", "3bbb8a951e3934fcbf3ba773c87a4295b055b6f8ff5003377339edcd2b1b10d7ef555e3b1053488ee2c5e5e4e1f0f63aa0cab30e7dde68afe40af8b37c3f670e" },
                { "ia", "c760e49db14ace136cf5950694c3507b43cd45a4d51104e418970ef15466ce394d9b060cab51c6eddb3bb1c1bfb0c1fbcbab1b1aba56a8bcddea6361ba8cf12c" },
                { "id", "a476075af94863a0952d8a5648ab425310e0a2c45ed06e4161eb35af1ec1086c479cda4e3e5069318d86d60392f4add155f1055eada3a698b4b34fc62871754a" },
                { "is", "dde7c713f8f3d7d6664fa6556b2659e8cd1f7e7df66293ad89a2313929b997dcc6472d88ed9821c5bf1adad2a2311a3e76d1db650d52698f790d0d3097b570f6" },
                { "it", "6a4c46f508c086e1acda3c9c4fad2f3aac61ae64b2a39fcc58edc870f383ea3b49937b4e56c1ade933cbfbb44b3fc5efecc9b9d38ff58b28d4087fd8ff977462" },
                { "ja", "46bbe70ec7836a302626194bee45fd66522f00ce7f4aae965c971acc92ed4b55ced133ca00f17f436f1de2e5836c98434dbdc9339dd6603ce7db2cdbecdeebe5" },
                { "ka", "f39a8fae2478727e3f49551d8e632ad1fa7b36c4bf548752396c9e3130c70bdf9140204f4c12b438f4ba904ba9f6645d389a5d535c96a52685299fa3cc972245" },
                { "kab", "927a60c546022b3b37281e8fb7387bfd0cc478f4d4d4ae0f7b53b3bdae7a0aea1f9cc19e7772e1a4233b31ed0e90ba68a679b1ce3bc5c73f0df7ae666999a118" },
                { "kk", "51131b4364c6781fa034e2e4b29701a1e21c10dd36177818d53f7c7a5bcb352d3c722449f683d5034a1e5a9b822f52796b1cad6d064cd81ba4984d2c8541223d" },
                { "km", "8c905a8bb3b3ebe3fe5297fbe1e186b78ebab266c1e6e4adfb04516bac886395ffd17de3708428e6d2951ba36175658cdda18a8feef143f5d6a167004fc99d65" },
                { "kn", "d8f17f5ff15ba5c8c76a7ff0d316d0ac725ae1c8fe0f345106c0b7af11a6f5703cf8edec500ce339197b8bed010efa0e8e9fd05840530a466d6cd90291278af3" },
                { "ko", "7af3b144ce1f4de106d11341a523b19410525373a4d60aa8f2a62c5509787ce61d8742bcb6385e98219ba2468f8b24add295b7c9b5add2db3a75c6a05ac88159" },
                { "lij", "b3089930740a086d007811b6d659d33da14e6d7454f0095a5f4f070ea2cacba7b2e38c35f917957bed4c78f65dd0df942b51de116f243706fe635c45e0bb0cd3" },
                { "lt", "7d8b697d0fef90e94f04e37b80e969391a99cd8b7e3517400dc0ea0facbebcb877bb310017c340da758b5e0179c75d1cb5a3cf29397fa3bcce967f7293981818" },
                { "lv", "ce888347e35f8151c0834634b98ee481bdb53d81b23dcd6c9ccda0c10fd618853bbfc5ca08463988408493d2419c529808adba462d6acaf6bc2cc9ab693a6786" },
                { "mk", "1d5b9f0e9e7a6e2c675664f50d5fb01a9682d56c6b909c4af7918690802af13251fcb8a87505d3ba3c8e55ed18b40d376111c2cdee2d35d761028d9b14cf2443" },
                { "mr", "0263358a8e3d31b118ae1d13a6218e9000174bfc4d64c695703deadbd5fd0d5636fc4e92a8a2feb1625072f06ba6812ab129106f4a13ebb951ceada3f3e25844" },
                { "ms", "1df8c4fad1db3942360a242392650b23c5f53117548e1bd89096734b8a1d8a91ef4eeccf1d581f6c069f81a565dbd5c2c9adac925b136bff85bfca1ddae0b681" },
                { "my", "d4ebb9b2c49e696cb32d0fe7b97db6653fd25741c22d96b8a146100f27e145bec916b5c3545186dd81b039db9d0158f3c34c2cb85734fa18262859cdd715d14a" },
                { "nb-NO", "c41d355a2c1bbd3c4322023c98849fbdbe6b9dc8e823aa3641bcb41033f4b67b8850e3b2f0fa62f2e883338edb8eada86a86120691c76650a01304b32d9a40c5" },
                { "ne-NP", "4faeced6dfe217116005cc2eaeaf79e004165354a473c7f8430c77b8860d054ecacd4ae73e3947b6c8c7ad6165da88ce7387dedca427e6ec6af3649d9e6ac86e" },
                { "nl", "f596f1baa1776783248f9701d4a18fc5f6ddda7bd5a932849041298d5528f2acab7ac31e5bec31cef5173ba4bcc1a9bd2fca2983a138013a377b67507e7e847e" },
                { "nn-NO", "5d02e7e9b2c98593a535666398f23c2cac5ce4ed22990573f052e27c5642e28ea8152d3e1115c21b967dee702ce270feb852c82db5dbbb128c9d1eb7a049d839" },
                { "oc", "8af79f609afebe5abd9851bcf099cc3caff5cc592b3b9d97edc59eadf4b9adacd645d3cdb8efa54f9a8526190946d3d3731f1949ef0410e4083737ee167d7798" },
                { "pa-IN", "cf55ad343c79ac2fdc7b7624ffe45a05df34d521172108a2a6d5716562733321509435c44f776d636b944bca86e00b16aded192c2b643c2839ab4d579f2ab818" },
                { "pl", "04dda2cbe6f0f47520892a46c9b706823474a2626b9bbe142f693df44bf2cc25461688d711229c9af3aeb77380bb8eec2326e22ca3a7f48932a70d2bc4405d20" },
                { "pt-BR", "1b0cfba72fbf80a778115dd0f1678e36c1c51714b4ebac73c9d81cd80ff4b48d0e6caad7fbeecfafa48f7d0f49e765388455d09ef7aebc4518b6930cdc794bea" },
                { "pt-PT", "8bade8d29e46f85f15fc66ee05ebc33a51f17ac3e2669e26ed531d7a614bca578b0981c2097ae408dcbdf9c32077c0ba967e3ebf1f2875af31096d980c71d0d2" },
                { "rm", "42717f0f95c4c8900154617681171c2c0d2a952e95e598c562c5c1e1a916cb23cb619f7922e6d5f96ed7f89e2be856946c04bf82cd63cd5c4e8560c8c2693be2" },
                { "ro", "1b37a8032d616c1dc89fef11c3e9aea46018ecafcf4fc623a4158391487c3cdfbf7f5cd002633d314ff04b5c32319cf88f08207568159d85c2a5ae885de7361c" },
                { "ru", "47e5d2661128158d8e5bed5f40ade6c1354259d52f9794bf85e3c4dc8adf44959304c36cae650e7afaa41adbebc22242dfdc74761cc6007b0dd28926d5c1c702" },
                { "sc", "03a7a3c85f40d692c5f39c004c60bdaead3c1d486bd225f3bef627ce5c5efa68c1365fd8d91b18996a4a671b6e18f4d6c04747ffcee009b5f4a17ecdeb13e9ad" },
                { "sco", "cd6b2d334cac5d1a71634e0af2dd88a69ba7177154172f49b6f77eaf584698ffae7eb4e7e886c33f190327660c6f5686af5de64a2c8d7bcf9671667685989c3a" },
                { "si", "762082628a4fc695dafef914dea1a19b6c17fcffddda4bfb0c5fcea4b9076c630cb7609d7c0829cb2517d87c35d8cd946056ce0bb4af9d372f5c47d1b0348073" },
                { "sk", "66e9caa02d11ee6db02d62b6b200338fcb9157d0f469bdf87a05d911fed814ab007aa74611b0e8929a282e954f278da1c04cbab6bc9b69b55b7197df5b906b86" },
                { "sl", "a50979db0d312dcfe1633eba5095fce7b4ddf0f4164e05acc012dbb315354c24a4a1c785526652ae8c9f83e7837ed395414e3693255c8e1842cf9944ba5f6f79" },
                { "son", "1c276840da0934345d576dd6660420dd56820889fb2d0663a8a442fec35dbbe23c4dd06a9e321734f16332721b59ec76c461a1f67c2ba01408b3a4a1f12eafed" },
                { "sq", "dc5b2cb2f7d7be3fdd02fa27beb8b37d4ec51acbb38288b96a6cdf0463c586f652f292eee624ab875f7a9c4d5b9052ac5d5c8ed59ae1023a9adfda46582b58e7" },
                { "sr", "8a6757be59c8d41e020ce54bfebe8bb65405d88b791c5d42138a7508d60c68857dbb5a40516f151d07223338ab8f931caee9391255f0c8c899d1100c361752b9" },
                { "sv-SE", "a236f5e267d8c7b80714b2e39c75d2bccd91a5487ec231ad820fec298d61c52b75e7c7619f2d1700b284ae26589c03346c9c3b48ccfee8918fa4d89790e9d290" },
                { "szl", "e04ecdfec06ec19ed504b4f21919298d6977b5ee2a908bc12ff12cbd43f250d301644d41caa78fa7353a23df1f71bb5fdf3be964ee155a80e95b65899fb4fa1d" },
                { "ta", "18607972625f39dd8b42e3dbcc769de17a830031632fd6bcc03c8bbe971801d65d42ed06ca229dc2c75af99933667c794f74f9f1a166a3caefa6366aa2abfd42" },
                { "te", "edaa37e2b9b6b3adfd04c1843d9403a4bf2ce4a3ecffc676a617e4e4d4183cb9a6dd6696e243eb903da80712f33e0464db42dd4c21fe8cdbc9097926c1e9b15e" },
                { "tg", "b68139dad09d2b77a03027223f3f01009f0e0dd3ab1c66cc70544fa4f864f8fa2ebdcab7163eebe74e6ad1bd39b929c7111c3eaf1e90a440b9cd5fb1c53e23fc" },
                { "th", "d6a1fbe396c3a75c6cb81f1faa60cf6a806b642aae9eb67ea6d1d98a28145e9a0b097859258bb5e45f35c11a74fefaf471002692fa34e6bb335b4be6dfdaede1" },
                { "tl", "a23ea3de40ae85b51b603f290e3f53ef34648c3c1c390f35677d20323ff9013cad41e3c38b3068dc5edba710833a55b28c1bda77ddbdd51fc48ee77793765b1b" },
                { "tr", "b6b0d088b3eedadfb63697bafc6c35de0aa126e3104bbe6ef9f8a1058eb88a1a8a86bc44fc9052dec9a2215f9ae51c0dbb793efdeecbe8abc3fcda2b816feb8a" },
                { "trs", "115c7b5c4d2aacb785b570f70be1fac34b415e68393a65cafb2210f3393737b4d0a6c96c16b6564275f3a9d0409b79f7791ccdd806413ff9e8a4f8f7e6dc2191" },
                { "uk", "05c71a8b4d0ff6a6ae4d3cd8a7fb3d29e319e882109248e8df2dd35a6a0fd60c9b230c6d12b064639b90099f795328f8e5968fc7104c2adee5a78395bb34348b" },
                { "ur", "b4ed7bd7090ce7cc2cddf9e0c5146510babd51bc0f96a7f668d5b79c6f8189f3c65776dea7c0a42dbef154c486594e05127097eda28bcb20b16e08f3cc3f8abb" },
                { "uz", "88566a198c2f5adc770b183848e279b5738fed366ca425c76a8f157238b871ab80d6e005477484960f229a69f4983f0c91b0c58757b4f56fc03da63d958237cb" },
                { "vi", "61c837aef092447d1778c49926e9af94ccf279cc8b19fe535c4c759de390536745546025b0104de41bf0a2f4193a63a2a09513441dcdf854c4e2bcebd3be4d93" },
                { "xh", "81103d7a5d417ca4468a2e65e7d765409804fb3f5c5439cea19d3fa8f1e837c2845c0208bf562b146676783707bb4e28b4e9658831bbb65d2120f685394918d0" },
                { "zh-CN", "ddd03dad114f6c9d48e786677396eda36f03703bbddb0a5a2d6637275501cfde2f86d0b77636f40360e97e74ef7194b03e58734e16311b14c45f5c57fd644c4f" },
                { "zh-TW", "a38e360e9e966d16ffac6bcdbf007fab797707d1952fa2832afb141c387ad4fef69fe4425b16c77c5b29351e4629e4bda24b977309419658ea77d8b448bf6bae" }
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

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
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
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
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
