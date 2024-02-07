﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/122.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "a80889c062537ba78790c47656d9c1fca9361766be770b583e392cae49716cc9535c989d644a0a98d1195f58850a0b8171bd51459297a4bfacdd2a56602c991c" },
                { "af", "4ffe46f63cc3d2424ab9d5ebc95273250e9d217e40d203b7d134dc6850ac8841adfacc292e0d654a6a7f7001a2d55c415eb9fad8e2cd9dc3ab6bfa94fae4157c" },
                { "an", "2d1d7964ecf6775134291031f68db769f4bb0742c4e46f5c93fee23144c32024ba32b2ae378e001a8b18fc9f531f83dff76a596bbfda1e42a1a3fa69c96e509e" },
                { "ar", "ebeb90c185ff6efc7d1b92ddfea4d8dd7cb03bf0355eacd6dcccccf2d76c1223f51b82eb48f852ab55554b64ad0e8d43cb8eed016c45fffa4071cce74d869ccd" },
                { "ast", "cbbe57cb7d948afd0dddffc391200351176fad8063feb782b331dcc7a801f7be5407d44d96cf0d891f10dae56515ec34e01f5242ad716fa883fcc31d57de6d58" },
                { "az", "53c00387d77de1ca436c34cdf3868e45e07885791ecfe52a19e82226f9d1c9a8fd9cd7ce196fb1e0b48151e0c8a67ba1e1621600ad7d1e06bcb5930ed7963e77" },
                { "be", "d9ce1ad805f85fda7927ea10e2dd2d98a8b14aa8052e2c1ed5ed558816144e39adc6de0a50e371d535c301976cbfa8084bf90d0690d0e0112a1ebf283337efaf" },
                { "bg", "e1cbce9f8862abd8289abf636d9960c53dec6f0f23b458bd9d0f9ee362efefdcb2b28d44626dc4ea0ea2ca9e9ba0d530768b56f7184e7208f8db0f63222bae8f" },
                { "bn", "39262af8ba8d4504d6d332b573bf4f36e30c8c19b098aca9e2d9830f665d764b18c08f50cea3bf09078a0ba1d337a98fe7a24a26bb411662093bcf557c46f9ff" },
                { "br", "dde9c67f7c269b2ed13cf6a1b7f884bdbdad7bec8178c240e8980a6e5bfce15be069080c80f6ad64e98c630fdc8902000f50a7c95f76f83d0ed6c254809e4a66" },
                { "bs", "b4940c2413be110c686d3a576eabd46fdb48265ffd40d0b6e42f6a259bead05b12072f0caff7e9d0d4c91479263c27374e59f2fe1bf849140a0f0c8c5e10445a" },
                { "ca", "8765a840eb7fba1c8562bbcd00cd984a9ba13519dfc4d62019a6f9e3a07eb0d0a0586226d5943807c40cf688abb35ecb7d9146483275143553d977f8a476a887" },
                { "cak", "db6ded1c4234384e6ab0725f72f7b099b6f12d517d76460243cadc7f492f30422a15c7071b47fbe73741db57e40cae32893e7b6f061278ff0ff74f724c4a1c7d" },
                { "cs", "e809186e53b91175189ae3f4de8fc663d116002697c81875d6b45d1477bae787b727e680f811b12972f387a35d9c11772bf9c16abee246a692dbf12c133fd4ae" },
                { "cy", "14a72ccc0e15371721b5bda6eb8e5ab5e2e9e89d7a9309c9470aa0d689b460bbb6d428fa2b3d4f609df209ee37be5b9c0dc5fa6e381b0a86ed02397c28e5de8d" },
                { "da", "241f9af79f64da8cb159cf25311e157a281d2fd0194f66c6ef60115ba26e23ab040887cdfcbfd7459b88d79c7b6a19eb206660183ce8d156c1b2b9fc9c08dc09" },
                { "de", "86e927b4a8b795e03ac9386af25db70f4da523c982981123d9874ee3882fc358df1af44c55ab10f933905b83bb977a070c1c065eaefc414105211afcf12ce624" },
                { "dsb", "4afdfcd78fbd7d440f1aaad3ba459f95850260a3cf4276105351b5c4cb1e751768681523faa6d24d7fb2303c7034430179a16402656c61e52d1ef20ad1383e2e" },
                { "el", "e9440cf6dc7947aad6bb39553ee80523af585cc25c321190c69b1979e82700ff212ca2a2de4f0a259d16df476dd59e3813fc214453ed68a20c86c25416f1d6e2" },
                { "en-CA", "b162be24659c08f30790f7f0a786b42ac27cb0fe0e69dbd522ef3ee943cca18d710badc46326aa28c0b737fe421c6e6b171b5da92353dc824c36f0c49006e1a8" },
                { "en-GB", "1d03793cc6c10da3395b1293ff9c4162ace485fe50ee30dc03b9851960a70a72d432ef31eb2226880fdb6d801e39c665439e7dbef18cc3c12240532c92aec1af" },
                { "en-US", "83d3fbc9f7ff69d085af98286ad9b09ea07d2a079565c90e282b15a15f0c84d0d1b01f7205aee34e2f280f537a70a5706d38bcfaa04aa687dd489c6b197f98bb" },
                { "eo", "894c7285a730d7254c4d7901a8301b7403f6f089712cf612000d9d0a1242df5c581709c7e7a46d77ba29f7f53335c2dc0f0f11ec6171b26f2cce9d9b5ad8645a" },
                { "es-AR", "2a97f7ae9942e92a3964e8721ca2ccbedddb926b5cc20f832a199a62abf059c3f9b8e9a9201ddcfe6ebd1e36b6ec3f9302776b77c73225c1cedbda33fe21af58" },
                { "es-CL", "47a6825481c0684406c6d4713db6bac3296a6bf7006da0ae75bfd14de5a8a70e8f0d48e15ae12e9dec8cae9ce35a930e38b70466a0aaec4becfc5f711aee151a" },
                { "es-ES", "538ed059ba67e2639f2710755cdc0ddd54778347181998c99e02d574efdc4b01c53c437f05b3276ebe976216cdfa3210f11827ee8ada98ecb1bf05af8d0cbdec" },
                { "es-MX", "1cc38f2b706e139c9569d1dcad41cc5039beaa0329e0d20f329b0b5e36b562b1745ca6c71798493f294f18ae0121b41ca8606f840a2bd572ad0fc2eff83c9888" },
                { "et", "dc4d209dcff4a371f31e857f5190756778f48e14e61dcb7446fd8de021ab75d7c4a0cfe2d03d415651d669cf721cec0ffe0c72875b65290df981a1ae926a3022" },
                { "eu", "888409d0dd05fbf9ead40dbe7810a8f4df9a9b18ff8649f28eb85b86f5c0ea08f0c5c967a34b7ee882df8a63700a4f5e216b92bb28b0e3a62e8ddd1f12eff9e0" },
                { "fa", "efc45bab7c3b8ccf4ecec7c36693a04a1a4fc958173a5729c18e19bf451edbe84a2e6b8638df4172360f4ac7b0ec204b0376ba7e67d2ee3eb8ef139fb18ba63c" },
                { "ff", "54135839c774b860ca8d556517c1667544c1559aafc8a83c4406f86699e55fb0ae2de0f6636244bb7668983cba493c3d217e36bc6e6d72e23ca65281aa2386bd" },
                { "fi", "524e7b2955962bb8bdb09406700235101fcc57a38e6e328f506dde4924c8a687d72fc3d5e1996fa806a98d4332936796610ab70541cd9617fe4692d88a912f74" },
                { "fr", "8fd95453632b480f981072c217d573465fe302d3178ce817316fdd64d0267d1d38ca3dea0ab137e9a613f341960b1e1ff68199aa291e9fabeee37fd8708e6592" },
                { "fur", "9ba7556291cf7dac1a0d020da3e192b5336359a6f29c33903026482eebfa848e73d508e7ecd2b4459049bb905bbb5d7a82b843882b272a167b20acd2e29dacc6" },
                { "fy-NL", "c56ddb06026abfe7ad4cb502310083e034ddf6051d9249a454469e3bf79ae5536f05e33c32f14ae6f4465dddb33391f0dff68402bda60e9f9936ced11843d380" },
                { "ga-IE", "96c89ff6db4e1cfacdcdc4745b0c2cf85df15a6b6ece04a909fa070c539cc9dc7e7a86dc33c32f30e8c3d023c8f5baf18bd37296f0540c6d56716fb97ab7dd9e" },
                { "gd", "5d5d559e26404d6e70252779fae54f933d968f2f6c4f1ed4803b82b8d9794e1a950b00375eaf6d7692f8195094ed4e47f90bdb8cd79eb2bdf14c4af9776709ab" },
                { "gl", "061443c325a3cf05fdfd4db89a32ee56567085f4d0fbdbfc36142a431dea129827b758a8ff4397bd2ccf91975cb7b792267f3556e6582f6249784dda02f343bb" },
                { "gn", "00297be82ca4f35239f5f8433ee2ab7d19ce1291ab3590148c300031f3612a892167c4e4e4c06201bf3ce584ba304a32ddf94834748133d68fdcb100dbd3adc6" },
                { "gu-IN", "07ee2bb8947340f909ab30d0eb489b9be5751391cab7bf7d2cdc9669e0852bf7ce0f200717425e86d090046618f00f660ca172ea4b16f0208f793761ad63cb9e" },
                { "he", "6124f3f17d52172bfdf2c74f090de663c9c080562180e666fe8e0a793e1fba70ce95d30348d9dc73b98be40137b8827eb5bb22ff0adc389fcbb38cbb76606b61" },
                { "hi-IN", "72f38b2249581b979987de2c9caec3230a821e6efe30ae297962cb53447203ce24faa812191fdc196252f162e60dc5f6836f1106c79a87402ca863de58af5bc3" },
                { "hr", "06a96631058ced65cb04be818802fc44d6e420c3b85965c6674e3ef6c2407f61c6b1a4fe6277806fb0e485098be7027a0fb0344e0fa026ce097abe8d04103e0c" },
                { "hsb", "9875a66ab0361ed12e2279d3f64e6d440e919bc90d59b5642a6c97b03800b16d5429152ad26f97cb0c351dfa47ecd2d7960b787e03427d672da0035e538e23ea" },
                { "hu", "f22f78c0740901c61d3d207860aa3bd9053838e0b77ac272da4ebb1d2af151c50defede821d536065d53d105c175f4c011f0a26c5c6a3b58fcceaf6329550a30" },
                { "hy-AM", "16abcaed0359b33ea6e9ec6b9b10fe14f4682cfc62f85355f90f441c71d7b9f35cb0f55903ce51cb72eebc916ff59dc2da4ea144f1f7a2a1e4b7c5f0387183ac" },
                { "ia", "160e34943fecbb5a9673fc0c5c45b94b07af9b268857db51a8fc853851296aa1901832c394bfee1b769972a0d9f1723974af20a0cc49097b532053df0937f921" },
                { "id", "e75e965e69546674f3b5bbcc31406190d673f8812397a74a2a1af79bf83d142340546b64ade22b4974e4491eca7d3cfcedcaf803e8b1549857a952c9a3a15d3e" },
                { "is", "52911fe2b17b1cadc146205dcf75ebd2dfc74601d83c8ea88974d9c975fe1f24c96bd41f351497e35678b24e3ef68b5d0d3f95c8917ebf5af35a96ab9f878d2d" },
                { "it", "094b7e45416efd746fadff92bb19fe0b719c9d38a098ed0f6d196d0ba0d8c64be8384eab75e4c71e126ff3e36254f7a58a10d88704540781dc8ec605a5abb162" },
                { "ja", "b5fa57cbe9ad627ec008d60dc7efd40ae83af29060ecad8be37a178c57e00d818cc9e6c41825f9322ef46e616c417d9d531fc1f3721f8021352f6d648f2c0fd8" },
                { "ka", "238e3aaae997f4337f46648aa7b90267d7061d0adaf33f97d961fb1e8707a811720def923a1f6a177195ec4470877563d08bf9761f3edfe603cc2cbe0990faad" },
                { "kab", "7c6938a83d49c853e32c23da58f119868939661790c4c83e3649167fb3c984a915d84b1b3400a86178b1b74b79c249091da970e5900cf0293f21e32791d98ac4" },
                { "kk", "1c43613fa3d512ebc915740f29ef18d19f0b05f248de9f4c9643733a2f7af070ea766cdd83f9a5023363bf4d1a9d147aaafe8f11333563f38449a691176586f8" },
                { "km", "c2c70286eef1a0422713246fb1bd07d7ddd9b8672a7635074bc8b7be646b06cf722c35ccbbaa5ba736b643bdbb611d0c1988cd4a6226ca6be815005f1a1b23aa" },
                { "kn", "a395c7dcaa6352008b72807c505a82aa8d0a52fdfffe34d2ae9e9762d9261c5c0b8d489092e876de0886fee038b74f736e98b2262c2eb7bae6eb9b6126cd86cc" },
                { "ko", "3dd24c3ccf984a939cfa83989024ff65085c168ea49b7a52252b26def146643578e7c2705b3fc8d93eabd81686fe813ca058ea582d799649712d7646e89f0d47" },
                { "lij", "15b382e4049d6b50dc5f540f306e02bb9115a200539f0b1ed4b65d867c92540142399dcb0569828e40d7ff7cf310bc5a48c1e36507c33f06e19ffb0d31baf05f" },
                { "lt", "590a74fb6d9cd4b211aa134081508090282e1cc3f4c81aec2b9358d19beb7a5be70278cec0714a200691f98760fc9671787a61861669adecf5e7741523404097" },
                { "lv", "e46763bd6a1c6f21d80d442854f4ecd8bc2608226408c90775562b91989191e83d43dcd82ade9c2401a89e2aa81623243be986e37dfa4fc7cd3d8bf6714fc654" },
                { "mk", "21d81fa7342895cfaf4371f4f30ff4dd52334969fe6c3f6291299999f9137b8a5a37474634d97d457aef26dafb028b743caf66a80fdbc011ece39cf4a9fa8d7b" },
                { "mr", "24761efd1a948eb03653d3da6a8a2e16b77ef4a399d6d546a10185bd4b7ff590f25ef3767b55f797f5d2a579839345a58c18f4d22f23dd20ceeef0044a5aaff2" },
                { "ms", "018370556e01649c4a8a664596c604953e941c3b32fc7c3ba437371ff353dd0891cf4346b5ef8e14ec7430fa865a3c5433e03ade9ff257c5d4e03bc527845be4" },
                { "my", "3824704b2ea0bea820fb5670adfbc2e1243c23a2be319c8a34840b3ad45ad4cbcdc7a743d574527bbc9464ea33b9b1a800341f1b7e109da452d6427e8942fb7f" },
                { "nb-NO", "aba7a299f947afe737cf0fe53e657a1ab896ec6a655315b1306e50befc9abdd16bcaa9fedc44c2f9555f5754c882d15a1ab6bb146692fc9826a9e3afb57b096a" },
                { "ne-NP", "b8686794276ddd1a5a211e52fa3fb6301f5af7d89fbaa46fcf15e1a755aa9633c28deea8c0e38b932872cfca3daf61030d8e3352490c292d2725c7351d2fcbf3" },
                { "nl", "cee716dc2d085e821ecd55e373dfd50e4b07030eff66f7f3608e715dd2cd58e89e9ac023760c8737c9ac3980df9f339c47fb4967c70b5329165b20fb364cb3cb" },
                { "nn-NO", "5b4ee6051f0162f08fff2a100640fa68cc632f5dc89b6fa4d467bd9b3f5d104add2baa9314a9033da028f205811dfc8e6c36aaffb7455aaa790fc5773ebbea22" },
                { "oc", "d6a5cf588f1bd12b89a972b623d9375317d3cab4a459c29c2cce703521e030d09733263cd49a81fffe70618e5e17ac0c6fb42e0442f86b4d04b7ce0f7150bd8c" },
                { "pa-IN", "3abecdab8d6ceba8add5b869c3d982c9830d43193be5072af620ff98a8ec51e250ac6a9a503fa35fb0206736a89aff34b1be2cc536607586895bad566e25e6a3" },
                { "pl", "a1b94a73709ca65263746897636db20093e45dbb79911d7994ddd43715b3748b5be663d9805e604589d46ebba7404980db0eda258d41391589e2a57ef095a6b1" },
                { "pt-BR", "3f1b89c66eae7e119ac3e01731200f87107699e5dbd89eedbd0d85c4df57787c209225ab8839e2b3355e4b1523d534ecbc1fe2aa632c14e1b7d13574fd97f100" },
                { "pt-PT", "f6da87abdd746f08dcd1dc27041e9b33be0010d79109af6182020125c8cb8bda3c335db940ec0397b1a9f54e67dc9dc49bd318a9841b88646a88ac2cb69c3d6f" },
                { "rm", "6d7b6301a082ba594b035d57c24666a46281d54d97571a24e5ddab839b4934f221be422d2b578a335ae1d39b9726b8a02111bac25bf4e6fdcbce59e3eaaf272c" },
                { "ro", "fda156dbce0485edfe15f09a90ab06ae60be14f9b37934ab6499549a51161dbc61466c0df076bebf3bc5df2b3262207a8d156803f61bfccf0a82442b0e1d7691" },
                { "ru", "e6df7e8a2812d767e785a2a925155519cd87ae1191717366217f415846af89e9e1c3a527409cffb478637378f6b2516b6a940e1ad823b9bf6d07dbc7995e02bc" },
                { "sat", "be8884ccf8ad761b59959b86b56d6b6693476460a8da3fdfc9e7686a4d20f200b7225dbdf3f85e52cd752fea09fed2379a272730a9b0c229b954ee992cad5db8" },
                { "sc", "3b575905324e1b16b7464ec23e31a261621acee9fbaca7dfd4373a80cef401267752636e22e87dd13cbc33e87bbd543d2c7fa51c9e311a7bd93340e1f27ec482" },
                { "sco", "a09403519f96c3cbbbc042d442d40a9bd52e25012c77b51870e27b99d62bc0c1984ec512aa8afb5762db8c7bdebae6fbbd83c988dba557cd98496228acf365eb" },
                { "si", "ee22f44197d20e8332ef70e1f6697b71157a9df6d713c0623f5d9dc4a19b8b3afaece543cd890d524258cbf48a99aec039b6f6437ca89e1814110c91e2bc9615" },
                { "sk", "45f456a63a615f3f84a86b7cf1358ab2cd3ed98c083eabf62a07ca2661634e35901caac5eb7b78b760a4d1ad94317d8d07d8ae61038851126bebd6d1e4cdbae5" },
                { "sl", "c0b04558c44db932577921c0cc2c6cdff21da62e397a84e357ef29eb0771e5ca487cb3ca60a9917bd3b42e7eb80f2131b2db4877e7adceb7ceddf39f7535275a" },
                { "son", "54334d3fe99c86566533c8203cc2740cf0d803900007dcfc74346afbccf2659888274149f716da6a8138d730a005ee9913da56c789922a182fcc773381e070cb" },
                { "sq", "b7188c22aed954f1c5214301fa1132ed9ce455b8338f110b0aa67ab802c7f48f50b6e56c3528a89d939ec2ca3ab84135e0f79263e34061c8c0dbb5b95f914421" },
                { "sr", "aac1efe9210e6d1bf49d58b924787e4ae2b07de6b46570348b54dee940a295130685439cb56d12625abf39f31721ee0ed0390e5166d25cab48f61d15e238a2ba" },
                { "sv-SE", "98fd08ca604eb1d800d28107ae757b2e4d6ab8491039aa2434f197c22ff6498053d257632106dc935d74da7ca68ae168bade3fa963bff6a2ac93f8a99c9b68b5" },
                { "szl", "a7d8abd9df28cb30c4db61b6769cf761a77b61cc5c92b214fcdadccef654065f575095fc0382993a09c4917cd222ea26714f3bfdc315f9f41c121f2be70ca4e5" },
                { "ta", "e09b40b178147864de7e10923f9378b3841b1d167ceb1b13e0e9940300b720cf5e32cb40ddb32cf3744db8be51a31cc22567e721961a25a5c2d6508a15b6b687" },
                { "te", "5140e5cfde4cbae8e9b6bb996cd2ffa0f61c20dced99b951cfddeb67deeb30d0c1127a0c45718e8e1f002ce887ac1825d481969fd217fe4a8ea4584196417524" },
                { "tg", "69df3844e9f2724071c5038a9442bea4efbb76a467422b7c6002987fc83b883af0fd23d4a47160a7b5164d1e1486534ae074f2817e68939951674929dc7ef99c" },
                { "th", "0a25742af1cfdabdbc6e7e7ff3b7aacf420185eca605169cdedd4568895eb7b743c0fc1a32a5cf91e17eaf96448d7061763b77f324cf3b3fcc46006affbc110a" },
                { "tl", "580b16605f27f119ee6321b6b0d275ded8ff09fadc0451c3bb20dc113cfe43c2f91ff1480dd4149eca9b711e8d4e4d456bdbe985588b3b35d493627517fe3bd9" },
                { "tr", "efbfb9e17f838ba40abe0161d049bcf13a390f367223661e7763ee14eae1446df25c6825a48503e88a8a01af8adf17a6c17ac257514c78002553692fbb167df6" },
                { "trs", "3bd4bb6a2daf7a567909165886981f1f88fe07a6d8195601dadeab4864bd030c014a1b0abf6035122fd20090b38c8c2b51c4dcf21a17381b216852330dc0361f" },
                { "uk", "fd3de6e75c359ab4c8f34f773e62f644ac683909ce29746ff61f06d53939f67b4fab05ac0dac3d3e6a477c18d649222346536c7cee3b41bcb14cfe81aa84f40f" },
                { "ur", "4f2213642cd13695e80ba04b16fb2d6ef8f00a0cb870bc488234e45aa89b2a4583b07f64ffc489836d11490dc7cc543ac2483c5ad46e47aaa759a3c6325df5b2" },
                { "uz", "53e3e01c5bf94daec81b3400cae99059a3dc8eb31af3b0151fa3a24e8d102703c2ce5b51d44913c42c8c873e3a74b58a5460f47d0e8918ee47ebec291da2cc87" },
                { "vi", "762db1a0b9a5cfb9334df419707e2960625beb5ca1fad9f28a4febde4b915a387591cb1a90f19940671c1ff23a1cb5d5d07383270de17ffd7f2139eeba4182d9" },
                { "xh", "0f93f4698c308859a1964b981a9f0b3d70304feb6591b8f731d09e9931a9090bc35e780d219524c05c561f17420d445f52268245aebe4f1586a01e695af85e26" },
                { "zh-CN", "eb8b0681b9a5174187042f751305f7c927564a7a28b5471bcea321c0762300f74d257c3b08d485234e48654b684104a9c2aad6b73fdea1b6d0a5289f10fe1167" },
                { "zh-TW", "bbb7d0d6799ecae5d07149306386cfc9112a27ebce8e15db77b853226357b08a8a82b164beb753dfa38867944676c81f71468858847b362ab8f791dc1076dec5" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/122.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "397108a92940f65eaa34513c57341d741daea3945a0c0aa739b04706cc7ced51927d1e8cafeff2265e4bc8037ce5e6d47bb3a810cd49bfe6d75c1892c814286a" },
                { "af", "6cd50d5f2bfcd22f50fac128948c82228e3a337f2ce354d1dcdd57d643835a4bc50bfa6d648e93ea8066fb545a40038c867181959334cb28fd9822854dfb2835" },
                { "an", "3a655439cb511dd4233a1f72ff6cc3c4aed9b36c29680cea441b840fbf193d6c0f0e4bffc4fd8eada0e66ce50fec3da909d9036f81bf4a3da6c2e9e9497d2fb0" },
                { "ar", "8bcf10a3c7b10ac2e0886510c6a12a77fedb82bc915fecbb90f942381f8e4a88456c2676766a4237b6c936bef5a35e0e63d35de74869788917cf9dd2285b7d9d" },
                { "ast", "b12bd5e1796beeb84d56528332c6f1f5a4e505a77228e907fb86849210783a59098432c635ce9c3044a7c4d69eda6c68d386eac6d92d4c6aeb50bfe7ef84e00c" },
                { "az", "d9b45979daaaf76f1067d683c6dadd71eb34428a52d08c7a03a1f20c87bc57719084ca7db1a96c67c4ff97d3512e02a8ddd541691f945d93c71247c9afaef064" },
                { "be", "aad15811901e08e9b2da7e08528ea031ed30ce437275e5d2584dda9de0df10b984ae926e0ab2c031ecda30176fe008cc0a537e4189ebbca8aea894ac906687bd" },
                { "bg", "e669fcddf2ae78d7f0033311a989806b846a6fa07e03e96fb1f87fe0999229bd40331d7c9571277571a963acb05c4eeffdc0f1c080980a325bd0ba7ee0b9b2ae" },
                { "bn", "da3947b4103be8698951a5bb71ef722bf8bd518492b8f7ad67d76d95f154c7b660e1f046ca96db49988fe7c4c8170ceb236168d8bc28b27f4ca1df448946c454" },
                { "br", "75346843d89672e2ab48c6100f9b42c6f06318097eb9ba00ff2920ccfd5cbbd2d7932901b50dbc1817b4422b1e38834e880599b0f80b041175b2653d3d075f3d" },
                { "bs", "232a50cb667554f6f6575b6cacf350c318c31d52dfef563ec2aeafa7ee1bc3cc2408d8a69480fef46590029f733edf10ebfca91c048a9228bb1d8c3c9b924055" },
                { "ca", "4843f77d706377a72d86fa76ce5d38f7485336fc350757c7793080f955286d43fb276919f0a9732f3a34cfe0662bdb29dc6e668250c8fac94e6cc5f6576f5c0c" },
                { "cak", "d9c264b1009ead6df661d165f71cc2804d3c2b1586b21163acf5935065bffd9e95c649fbde6efe2413767586d2f9201ae18f65cbd7c93104a18d806c9ac68ede" },
                { "cs", "d8066b2faafce10372864d51af64e31cc04c5c8ce22814a027073935f4625500508c21eac325a0b9a432e33899ca14c2f49682b11a63bb6ea541dc0cd6222eb9" },
                { "cy", "053ba33ea3f20a721bbb8925e4c95830223f33bb424ea918d380b0d67b2197e8e898676840ddd60636c39d30ffe6968eddeafa2d2e6bcddc71cc0b13b99328ba" },
                { "da", "0936c89f1c949940568b05b0afcb93cf0bb6a3b485fc8da965aeba4a606ec12eecf040cfbb8884384675ce86950bbf18b5eda5d15e8ed4e54b5aa9217f5f3304" },
                { "de", "b6e38771bcecc0234b469495e8e24eecf007f5f239e9af7eb4dae54f46d24731aeea4d94dc92117b5225349d747a2240d52b8da403def2da2d6d32fb2095afcf" },
                { "dsb", "5e1122004aa2b56c8e8850a6a065ba3979ffc876b7e4da2404fcead0a3bffccdf9d4b96a6b23cb6e290bf50f45df3a0cafa6ff764258aac9aca9d1c24105e6a9" },
                { "el", "eda1d167dffad24f4fa1db347c83c6373cd812ee6eb9aa8c9ce8c304ac5d1da37981ca4401e608e9c19aeec5c0e67b6445e84118e0c2f3792ecbb7d2b5b07c3b" },
                { "en-CA", "8035ec4c46cf21a8ff5cf4feddb91130094bd1e9feba43146ebbbb5687128229da30821c094dc8afffdc4a5bf3326875e06e8a572ac367f3f2b1771c764f9e39" },
                { "en-GB", "511af2bfa7a139d1c220cc6d21385d530a2a2d1964afb44766bebbcfed4385ad1389d089066a08136208bfbc034c2590d3ba4815c4c8c74fb61ec7be3a110d2f" },
                { "en-US", "5ed404beb343c55cedd8d2506ab432594c07c03c45133b6f796699a00fefea21bdad46b79bcb9828fb5e19294cd75e3819799c93bf538391b249c6d379f3f716" },
                { "eo", "be21f3e60271539bf2cf0754f85bd1d5819c5c6a8b00dc81cc6b476f89f67bb029d0fb612f94e89c3cd84dfff42eab61740a9d9fb07f08b2f5d229e9b39ca5fb" },
                { "es-AR", "72471feabfabd063444073d3d34fe1a296640d2b9adc218ed4010aed79d31d7198e063a9a319174de1239a78f2903346f97d61eed1fb934c4a0fb1e4918fb6ed" },
                { "es-CL", "5ec8222fa629439438a92a41d2b5d2db8a0149ffc7cd63dc1cb0153fbf47902ddaea20123b2b83dbc8cafd96da4f9058ff5db73bf8fd700f475b58db934a4aee" },
                { "es-ES", "86cdc09bbd25fb2131f27354bd071eecdea94c6a978508422ae3fe841d5781560e9b1c2e847ff7f1e65b336fdf6453bbd03fad2ef4a4642c2519274c46d199f9" },
                { "es-MX", "0b83a4730576842e676ce6c05dbdaccc655c203af1aa6efb49a9150c045bcb9414d1ca496c8193bc4af675db812c2ecd8183c35e1e13b44d59e85f5a92046fe5" },
                { "et", "2e8d04f9850c04a235f278f8ea2dbbd65e2f6aee3f9e5eacc5d844980b162637d30245a90d302a8bffc85f6fb2aff4e73e023914b568c5cd23f0f47155584b8f" },
                { "eu", "e022b10c1de72b58495644e2663f63b454ef112f66254adef1d0f9314ebd12277ca8be6dbe46069298ed35a7d9b299b83010ec41b0669e7a4689af110697a459" },
                { "fa", "e3b66f40cf8c1e49373e21f4cb0bba98593dcf3e25adc0a3f14902b00ee27c87050478914408887cac2175e8f77cdda3ef53e9aad3c5c68412bccfb774d6871f" },
                { "ff", "d6147211f9431058a0be03e15f68cd714de56b789e1945b35974588d42473c44c6f8e9d9547a558741d282766dec9a93cf56115b8abd57b52d7a9c5628f17d67" },
                { "fi", "e39f53fc35c8dad278edced71a3750692e117c7ea4e59a0993c07ea394dd7137d34c8f6d07d85ae245531a0ffdd585157dd4fdeb8afa642dcd7bad566875e3a3" },
                { "fr", "9735ceb9d159b1b283d07d990b610aee600881fc300a1de0f80cb0d273e3dd9aac6cdffc38e464a2ae877b24fdb4afe1a940c733a24c30a81c439fa95788f4b2" },
                { "fur", "4a369d25a71fe053fec1d8f966bfb57fecdc4cf8a1b33941a5e6808847bf83d63bfe813d46f74891ea99613156575b06ae864cdf570e7c1bcf8a6787ad3d53bd" },
                { "fy-NL", "7f9677dac015b3c90723a569a80eb12a56f41008d7837abbca028fbe7569b2b893b8b66b800d8e5ada15fb2b36721a4d08b549da2c77e7786bf82344b1c0fb62" },
                { "ga-IE", "30a83d9559c00fdf50e865b443cc203e6b002131ce2008da426e7e8b9310294000f1d341084f533bbb1ad6479755c18f7c84997e9a97cada2e6aba8d5d2777f2" },
                { "gd", "7f416538b5692d16f7a6e62fb9013b65b049f2eb50b7d8caef63e7edc2389df9742edd03de242c8f7b4964550e78c5783c610ffe7480b126f3bb54e8e06c0da6" },
                { "gl", "bd90551d9a89182889f37111be7f800cebf1997f18a5ac27cf794ffb4706fe93a3d7a6581ae077948e305038787d582e013f0dba75306cc2a0db8405aad9099f" },
                { "gn", "2070d24b75235b2561610401b21a510bfacf3b4a289fcbfb460b9fa8eb6454f99cd1a15433768cc659e8ff404f60a7d9f7432d01f924ef944b8a49c26daa64c2" },
                { "gu-IN", "bee9b1f589528f6c8bc7e091fe07ac3336f52c7fd15e9a44f2269e50beea43afd443cd1d46ab43554a5429b985a2e44041da8a3743fd60ccd7d9687b8a4c4abc" },
                { "he", "81f530cb9144c193b9be117149911e757a8f4ad0c9496851ecc72c0ea36619775554427fc69e35f6bfacff059ef841cc6b62c8acd2fd3dc52a8a5f247a6daaf0" },
                { "hi-IN", "8d8c5b26c6d24c5ed618ea6c749e0586aee5d4049543669269e15b444df714523016a67a49de9f0022a4fa9975039eb2276f6fda132a39da4237a59bff62921d" },
                { "hr", "f7fca21243fed0fffb3131f50200f0b6e65c1ccb4e1af336168c747aec0d70a400ebb4f10504440eb43adccd86936d5af30f8e7ea591bd3dd200efc5c7eb23a3" },
                { "hsb", "e0cc6ecc2a53bd7056bbabfb80cbaa9ce5dba629ef160ef2eacafdfafead1b29e978b3921a94f56f5a8818b2da7ee60a1a3203b814a9b58109501c986721ad30" },
                { "hu", "56a94e533757edb6b13bb0b54c71d378353458b8d81810c28738bc5199883ab3c38378cb86320e4a7d00b9a12d38989a4a005eb02872a93aedb07aba727b3659" },
                { "hy-AM", "3dec8a0853bc7bbb62ec4c08f460e5e3e1fe272425bc843ef84c0e27b78b0901a83548dc25fdf5062801f4586c8cebf3d9070601ce84d340077ffeca280d1361" },
                { "ia", "d203c567596eb2e30c99734221ef607a2adebbdaa549f610f1c7e989209adaa869ef010be6a3cd4c5abdffba650c547b405e18c92f3736066d2cca77e896c491" },
                { "id", "91adbf8e3028473bfe32cd10f8883b03698083791175c5f982f558d904d965f3f8c45e3647274c01950c6e7f5c7ab568094f0ff15865a65dba19bb787843c0ff" },
                { "is", "03a06b2acf0d9ab1c3ce906800d95f6bbd5e74fea734e590a5de137eec1459ca551341ae966fb453f9fce7b991dc894704678c8bfc11f96a39b22c526a657d2d" },
                { "it", "d4e9fd77bbf188d8bf981381fe6a7be89dc356db4903edfb24eebb3cd08887547b518548fc1ebbb4fe8414ef209d66e2055708e5e2de9734b9b23ebf9c07296d" },
                { "ja", "3dde8dbfdee7c8d9d7106008d3374e8832e1e2cc62cba9494922e5a91e2d67f4d305a7dfcf2f9816a9f4b0ed12f4119cf99229ebc234a7dde7521abce2826f05" },
                { "ka", "a6a53df51e44657be1962383aa6e6bebff3a5a9e4b82086ab71d820e1fe7d18dd5e7206af30ff71adc5658b16a3cb76626b8003dc25e2e10f8cd6e517ef8339c" },
                { "kab", "96f16b0f5fbb6d51bb3b7ea206f4f8521bd4704193d954ef546576b23fc6a2ef7ad173b7142f25d9e4e65ba7be4fa997cd0c1bc5c520b9b8c02b31514445db79" },
                { "kk", "eb9c6f04bf4cf7a172ff52804578441de49f24911c7a4cb403e6ab7f70f4f555d44eb06f9cf47f0f72edc9e34108899f2f3bc41bae03d9f0b18b02ff5bbbcd3d" },
                { "km", "ddc24157325fa2efa23c4f6d53b91ecd8f59c0ad8ad5f3a8971508c777eb324eefffdd303b509d7af863999fc8a320ce03f71b0614c04c7b272f7349f33183d4" },
                { "kn", "ba5f23c9ec69c9961ebd5bf40aee778603eb62cf2cc7b4e618280cda2609501d3c85fe01ff87b2b7a84774a52bdc50d0761f1b0cb3fea59c6ea355ab3dcc5ccd" },
                { "ko", "dba47b9781ece3ae582ac35f9e747e272c9de31eaa853a57535a7e579bcfbfb991e3dde5800ad9b0d95a299a31f25e367341f4897129dd0667efdd056a88c8a4" },
                { "lij", "41b4c879b19c1b3546e660f1dc57570306edc439e6d551f0dd93d19bf350e1afda5ca6a850ac1075046f60b2f89a6199f9bb8134b425c74460d160ae70e93333" },
                { "lt", "f1b3a48128bbb952366c943bfffd9f1e18b25241d2a2a747a73ca324b196f61f5fe3b3326aecfa0e3cd66b563c3c14ff77c368721c53f8c5ea038123f63aa1f6" },
                { "lv", "b7d3f0cb3098dead1ee3c4c55e52de39044f75e518129f5a2f3eaf50e5deef530941e4fbf3fc1017183785cd6f2692cf57ad21001d12e524b8ce596fe841b7d8" },
                { "mk", "08a3f955ec46a93c8705a907bbb78649191dddeecb7fdfbcb388fa974f48abf7b18d5c2faa49a9c669f2d3c4c6384b31dd1078f750da3c42c0263f8389e36e7e" },
                { "mr", "8a43b06b2dd7b0e7fb4abbe47ff187792b62eb53fc6246f2aefa86c064b3a992c48033fe48795cbd72908c566df1fcf953cd117f73692dd175a5da6014ace2f2" },
                { "ms", "d4982b82e97b744bfb7be03032db66995f394cf621458f15443815957c2166422b59c82a63ab9ea0ec59077df67c41cb06ce64d95305f2076b07f04541399b6c" },
                { "my", "66b660ba7d3cf54cc43af069f55928a2647cd84616d0ce8600d5b65d8b0b6b815a90e8281b1454d3ed577b7687cbe3b6565c120c447e258fb9a50d09587443c6" },
                { "nb-NO", "f12000c37671686f908926a5b85c2d6c39eef47b91b8fa14ac4aa77af1ee6234ce5898752230622ba843bf84b421ba429c76556d7aee64794fef8d82f0d854bd" },
                { "ne-NP", "5e89bb31a3e4f217830ec122905feb75016599016b5dd82536731e93259fd816c161a39bafae0269c3f3fd0686b402125efdafd6671c21b1c6da84da61485837" },
                { "nl", "840333f30c6949039f1d0234c187ea9654015aded8c80abe50592dfb1a67dfd2a4127c942a971e2907528401a0c2a22bdd0bfe6e5ca9cf36a539e1ebea4c82e7" },
                { "nn-NO", "4ceb70dae507814b8e32e88055a1c62fef48a8239308988a18cb87ec901ba9f48036931f0efef1e71e9527532b69d986e5ac35833742b1d2b7810df517c66dfc" },
                { "oc", "8f1073e314ecb2b760c093afb60dd215f6d23739a6ce0dd442f394f55efe0af45163257298664f3451dd42be956171db4416d2ef67ba2e9e86f7f591e887fa8b" },
                { "pa-IN", "17847247dcac8ee2c928075f27ce6037054e73ec182cc09bfb4f75abacdf633a65ab9011de9c97f2e29cfbf5af3047e33a9ab0df4ff6f6c107305275b18a2b94" },
                { "pl", "987de4e257b85be8e817a5e58e813644b431ee1ec46ee557e9e31ef53f729c00a9b3f00b3ebfdb20fe16d6b8ad3cf07e958e7e22fbcd4dc25bcbad243f65cdbf" },
                { "pt-BR", "806e1969ee70a1ed98e771e6fe57c893796614854caa7223cd8fb2c88c52e2de2f15586cf1cf16f125a10e7fac6c98c874333e83487e32bf96b65a02e22ee1ad" },
                { "pt-PT", "9694953af13f969e116bae0e8ec5bea0d49e411716dded1132215cba843c5f0106a86412edb25326b72492292392819649982c23041ea15da5b70b525891ed2a" },
                { "rm", "69123781255bcd5ea1c89ea59e03c038a732045b1ebfa725c08be60ebbd7b9df76baecb0780421234b066e24cb398f0439c3dd4af8f2703f86228cda5b16b4f7" },
                { "ro", "45353f4a9624e40d22b0c5758b447a59d834b5b501cac8ef6462d660927d7e093678c0e3b74d47f525d95684386af1832c0404e042eff9f7f2dfbffbac811a6d" },
                { "ru", "b2f814de80b41275b4c69e27d4bb8aba080610b9824109de260dbf6b85627fd1b8b32005d7b69858b10c0920837e76c164f2d37ac50c8519fb7a7b0e3b4133ac" },
                { "sat", "c458b2011db9bc32efe6af80eaaee8b09b9bd9665db2df7620996b855862a3bfaede36d1752c6e5b12cfe9c4ec3a63042242cac80ce18f955cce82873048fd4b" },
                { "sc", "a7ae0ed388fe3e1ae3f9c3a5772af5d707d2eb6eca5f5e78d1454384df04a09824a7928065bfc69f692c22952e0713aec8267338515d4859a641e28619921780" },
                { "sco", "eabe88ebca5d091024cad77f9a9e3dac9b356365baa814163fefeba071e8fa8fad4e9a2386454473762b67c73adbab56aa1476a5f40f0e38b7a827e5d025e172" },
                { "si", "1acbf2621e85e09a70db4baed6da60b8bfa644e453886593f4ef20d0734c0c0fdc98f775c38ab9a722d6a9adc78a7f73cc68284714e2f51e9bc0cc6f9f6c6d81" },
                { "sk", "51536484977df50192426c500562a870654f505bb654c131e6063db10c03529e0e35beb60180ca33efba0761f31ac151e1e9fd4e31e9fd3de4ba0b1d3c354595" },
                { "sl", "7912928b473afc0bb07130d4f6d207d7e0d6cab6c6a53a6f6fa2476b1926486b26e790c1b971861bf6175cbfd73db545e79fe61a26190c235ce38464eb0c789a" },
                { "son", "cca5dcc5488d3fa682336a0670e9d20c51f1389a06c7af3ecda2d88a7048a9f229cfaddb3401124048b3fd8a1a9414e44895b37550e2446af9d2c23f50b201cf" },
                { "sq", "9343c1ae5b89790bd07523bf584fd193c009960589b380f0338cd750f5af99350ef24550efcc0d00cec54a2e19af2ce93ec8bc5553c9c402fc96ecfec3575f7a" },
                { "sr", "a4e6822cf9d67b304de75ef73499678aaaf8ab727410da01f1acbb852e397c95902e8ec5716150ee5025867e7d6caebf34af5a6e6bdeba03ee362e2d04b87d7a" },
                { "sv-SE", "437bbe7d58305002d65d7b6ca90bbd45de05ebf5dec2fd105fe59ea405211a40651626301fca009b99002782a5776200c75af4f2275f92420bcca7802479851a" },
                { "szl", "3d6a0b547fb43fa751589b186accc91a9ef920326fe4bedc4d4a287a4d0b6b045c1902d83d07ea4a607439b8aefbb674d002c53c2d686e7dae7150ca180dceba" },
                { "ta", "56633431b13a9b38454e093d13e9b57e4d22d24af2238ae7d2fab5f4a6227aece91b639c296869362ed8cffc67742ec97e3bf02b3115662e0b91c8dfea964e3f" },
                { "te", "affe2198b52a5b6ef59af2ede998f3f8d1aeae1b3591a1b92f7dce13d32da1c46231094adeb5a4a83e493ac4accc72a07d207a10bf106a55225573659108859e" },
                { "tg", "991b428aff618ce406f06480efcc25e224ee6684052876efdd0e3e4090f257b9e4f1de9b2ce5b1fcd8756a817d54e7061f321adbc9d02b445139a48969d4a225" },
                { "th", "7ac22adb36fcdcee46c1827ea82ae62e94f8b71a5e63e84546aaf1be797f0b131f6669463a72a00e8ce018b72bf114d4a8aa5d7fc77708da3b02a84e4f53fac9" },
                { "tl", "96649bea70d447886efcf038429e808488b8bd38abfe3cfafb6302bb6b5fc3ce4838f7ec1bb49e6fefa2b0c534d6854331d9e83ed18ced247a1e920fcf58a667" },
                { "tr", "2ba68208c5eeca6decf20120bf31fb66b0bf25aa9a0e5ac003cbdb9369f8a2db11d2459934f2367df5bac03d595a5c17cba9f6efa7bf892c67b9775a22eb5012" },
                { "trs", "f8aadbc267ebfe601e00989f26a85935dd0d6beec59c212cc157640833ba1ca49bf471af718d84bb2662348888ec8b13ce2a12e4cd10bdb6590ce955915c93d8" },
                { "uk", "0daf5e604b66ce1ef806880613ff7cfbcc23d80b54335d4af251a81686ef34b45b45ccc75a83130bd80edf9e3f503144f8bf492ededf76039dafed0f8a1905ea" },
                { "ur", "4e74e485d323e0298e6d7201b82686392b9d97e80e0f21337888f1b1c51b0ef60053048fb92cf8e7911a9ac1fd8a28813d775aa73af04e71e1adedc01d9b6dce" },
                { "uz", "c23f86bc5462327a0bb5393e0e14154adee108941a6a574289b3e56b51ab07ec427edae4df1e1ec781e2837f406f1702b5e0a64b58f09cf1b3b8c88d5459ffe4" },
                { "vi", "4af1278d778251e90975892613b389845ce684408e56a0032b3e02011f76e4ae4797f07da072568d3a2113b83495dbfd7404cd8eaaa21ebb0579ff1853dffde1" },
                { "xh", "f933155910b8f90744291e4eb14fea998419d098ad4e934f6670cd5f1de7fd1c8ee3e983b938ebdea591def571070c3745f3b9dbc111f72259e68d217944b1e4" },
                { "zh-CN", "df28b85fb2d3e259128ef9ef02d9a676fad882644373f2c272bc24881df0661ee1f6f7ac42cd825b149a305a266a1ee14afd375856e6c2f91b3bda192f8d6045" },
                { "zh-TW", "f4325921d1986ce0067677d9c14a030241ba8ae0f5324738c0e1dad24e70d5d2b229e26bb634c870f27d4a663d315f7976cceb27c89c24744d9b4cfe93504d71" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "122.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
