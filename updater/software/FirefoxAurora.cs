﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "127.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "f3744724416c21f564a18c3f694dda4ed82a5e457e932b8d42df1777cd0bdbdbb4db2ad878e61d1a20f537a3fa0a1b2e5550d03ed542100e451305ee835ca0c4" },
                { "af", "a36118ce529007c91b5735388b824e4f2bdc3b7a32a5e5dba5855c12c3b8f5d7d4962a7337b394ff40d69584e8f3c9a6f34d82d7ebcf1912bb9e84b19e54d99b" },
                { "an", "78bfc273c0cb9c9dcf6b644df6d635e7a3c8af246a8f8ff70d33ea6859d74701fe67bce197c3ff4ff77f0c3f36a2de0939cca6668825ffbee29baaf7ed4a9a03" },
                { "ar", "ed9b465aec5d59bfc006ec62bf25bf5e96c4dff9a905c291611875b05368b37324f0f091d357d8e4133c7e9205a092a039f4f3552ddcc32117fb167f54872809" },
                { "ast", "d75ac47d3002aec6f83eafb6e806a99ab27d45f7bb8ee25f4aa84c3479adf86a910b191630e678c5343481bdb13dd7de7b01951acb747029346f5b378a5634c8" },
                { "az", "053d1c67c8b84c0badee95a292dc015c3db4e9c8e67a8b7e9aca7a4884f686a853b777bf9ed9a18a84988b3894049b4cd26248695692d3d28e537fde92fe5008" },
                { "be", "a972e884fe55449a52d82796af672d503d14a5b6df9d90683e1403c71b18ac1cc410fa910a05048b9767993f8930e8a0c2ec7990c007ac953a0d495db2e9d946" },
                { "bg", "38831d0b364ae659811e71c69c55a14868fec10359999333b40041d597c2f47103eb2122bd3b13445c5d772f568340b00720f5494822e5f0dfd6cb2a72d17fbb" },
                { "bn", "89139c64d89833d7fd4734ca9393b28d50465d5856a83975684a2bc11cd6dbbacbfa0e71eb2ce1d1a1f8089f603e03370e543c4f8519f18dd57a3012dc874a55" },
                { "br", "f001ffe8c7ba1287f9767c91ead98e3e0f007ca502a156b6c5cad12ec3e778e62a05666bfc6a89c1f8685a55bfa49a3f2461b9fe187827878359562c2fc64ea4" },
                { "bs", "38010b21042b5a16b1ac3d40ee5936f26cda529d0c9c2edf424c5e1af0892601292f3640520f58279b10ed815baef7857cb0df6a3afdab0e72ab01448c3c1574" },
                { "ca", "50c6f3d546b28e3b7165f393b1976142294fdc69b787cc263d9174de6dfd2b695b02f2b7cd270613a792445e44ddd7ca2fb1e40af5e7c957b3120667099e86d6" },
                { "cak", "895814998d74ba0f0c1e3bca899a1f54d2eb54a934c77e1606cfd07aca5cce017d048814874435a1001357bba751e2240faf4b14d74349980dc8145bc1b4b3dd" },
                { "cs", "2f428187bf2aa41eb05aa13d9cf3b67b9363ff7465ca966f05edb8aa2994810fd942cfe7524c7ec4194aba780759f586392c51078105c2f2f06e6c3d8bf09e18" },
                { "cy", "94fc01574c952a842edcd691681641ccfe88d16e40a066dfe78f2dde6a14e2198ccdbf0427e417e835a1a263e346debddeaeef7c08c05028e86601ff1fbab185" },
                { "da", "3d7ba47c8d7aa4791b53a2b71529f8f8841a472bb1bd3ed96978b39166c2bced42f30ee3cee0a851ca660762593d9bb2038452e1134d13dab115a809d100810b" },
                { "de", "0549ad28e8b5d330551cd07220bf340166bdf9dbd5f01468b34dedded7d32093b70111d2fba73fc43f19570eb0a388493a0c2bada21f6e99edf4f56c0b3a56fe" },
                { "dsb", "adfd0600e43535b5fb710e65e4b72db5cc269fec4a0e0439aed06bac305cea938795e93444f0e33bef1e6c3177fea84bad8c8fd9064bbe7c643c2a3f27015ecd" },
                { "el", "4423de7c0d61c776db7d085519d9fdf802722a32adeedab6a2ca0881a1c08333f345063ebb3117d8dc8863eca513f0c7e2fd8b315363f42afb29170d2c8933a5" },
                { "en-CA", "615c2b9a95281b12c25847f7b265e9254e53209a34d564c3a4f77d7fa414d97029f4c19a209d1398d60c461f5ea6c470e5d1ac7bbd91ec3f1c2349eac3f73d5f" },
                { "en-GB", "3697e4cec3b2eadecc2fd746fbb94e5712a4ede249d0a61cec70d55b08e0953b7663bb1a99f86fd5b58cf049f3013489cdf68f9fa531a6781375be24dc815da4" },
                { "en-US", "bc677182396af7a2a22821c3f595d8687df52d6ff0d299892d3c0425593db969254f1949ac854168fe9e24eeffc4490a44163f95a2c83a1da4e27bdec84335eb" },
                { "eo", "847fd59cef143c54fb999462571ca2db3b7034bdbbb511d49e63c22e8db464b41c1dd1c05e5e43ec8bb308a6eddf558b603e022fde68c625511b7897673439be" },
                { "es-AR", "cc5ca3572f69d2c9ed842c6a61893c89b773e9685e464cee58d920c8ed37bae0a67d176b3ec9fdf35300f902dc221cc9ccfe5484f118edea1099412a7cb985f0" },
                { "es-CL", "27a1f37440a97e4b816292b9ce318f59c9ba4f32699aa18a19a9b3650e67ac46cf8d05f08a229eedcfb1e9ab9865260f4bca7ad7a108ac5a8c51dd8135b0903a" },
                { "es-ES", "bca1962fff7db0e7756769af33cc5d9efd3aadb2d14652cf62796492c964984621cf6e279e582af279557f85f5f6a23679550bba38c22f974842e1e65e338992" },
                { "es-MX", "770775da3b82423f56a6103b53f4eef1213db2a0c84dc40d940e788a6f52eb12f99ef2123dce72d0754cf398dff1f7e6d3808f6630068601a35d67895574b3ac" },
                { "et", "68675385b1e00af481fcda795157ecd8715342c0d0fa2f28a508af8bcbc383daae2168c230724bc35bcf1ab3e87338f5927c9c8a32d19da1acd31f080ba91a4c" },
                { "eu", "fed09613e6e6d1f2f989da6cdb0cab8276afede55ba95c6e3dfff7ff0b5d94db6ee10ca8aafe69b4f6796f9298999c62f895654e8ec57be88914030c3c14bac5" },
                { "fa", "49277a71281067ea01e18e91e0f7a8884fa5f22cfa48edf0dbd01a145ceb71ed18a55e486126f61d1c87ff72547b6366c40e47dc0b0f8e1caa46b48c6eb304ed" },
                { "ff", "1e50e277cd9caa81ebac0cf617878fc9a298893260675402c60bc295e68a2fccee2e2f24fcbc4314ace5069c48cb4c9d0f367b8fe6a4995f890d7e3e4331ba9b" },
                { "fi", "0c9fa26301a64b1087ddd26b3156b6bbbca65658a51b6531d219357c4623bf5e79194dc5d746353a96e8e404023ba01075d9b39db32a96da783a45436bda3b7a" },
                { "fr", "71b015c6a682a334b0e3f51b4b89f998d62a5128c2b150873909800584322b5c643a3c671a51087213d19eb214771267aa98e3777d22887d93fc58e5130050e7" },
                { "fur", "e9a9316db4bddb8436a2ee52a682f2e627d364337a5ce51738987b1f3dcc9084147d099dffddb19ef0da787881d13695822e526fdda377c598f974316a30eaf5" },
                { "fy-NL", "9c947872ba09d6a3931385ad4677d419ac102017398d25f30e2ac745396f00da32cda81b43f058949dcd91297ccb57381342ade828551b22453f02aced4ecd3d" },
                { "ga-IE", "767a6fb232e3667c40c08573a2c2b0d2ade44590e2619cda0996145bdc13b995ffcff7a9b5e9c155d22cc2e00c3278f204192c0c45ee6e75719f91adedd5ad61" },
                { "gd", "db9de7a255465a64ccc20aeb1316b831d811849daea5094872ac65284685777665818316876945856826ac3fdf87d41495fa4616f2ee6748070c458e9d0e2831" },
                { "gl", "d616c5a83db48189953c3accade6154d38a334c97f8d4bdca0b394e306ad5d6ee11d845ea20ebd689d8159488c16c2f240379a980d30afb2df8bcd0f8d0f95cb" },
                { "gn", "2f75065c27ca5f02958fa0048502eaf89c15f290396d8d73f6479d5f33cd70f82daee3690b64ba5c6c442186354af322d36c481b9830e80bbaa4884ebc9b6d7e" },
                { "gu-IN", "fa26dfef3a61f33681574ee94401e65f8177376771532998617e03ec6dbb22f412b8a343d80349828e814c288871f142dca05c9ed864c13441177c026ceabcb8" },
                { "he", "6e318dd1eee965bc0497bb278a5c9712a3f46c9d8a1e1e27862cf304c6996dc3e2a694b7d71ca48d232c64888dccbe50e700b6257b2872ab4382e39db599736c" },
                { "hi-IN", "ea2123fea3ea07ef6f6cf606a3afece0943f925719f301e301bd1cf8ddca0ffdae2d373509ed93a430bf97ef7f296765201f9843496f6dc8a0ebed80e82666a5" },
                { "hr", "e4a1fd79472a688fa0b5071fee900694db32f54ba8cc81a5066c960817649e9d3294da289d281cf30b45840c4312516239b20094587151db855bba959456ec4c" },
                { "hsb", "bd0206a0462708ebd7a7c2db6113b9f103deba9416824596a991ca17c63895c9a457f7ec7b8860c98c8912d1e610bff2d58b6adf4a700ba2a2213af57f2a3655" },
                { "hu", "1c559bf72d827e0bd3e9ac2e7f39a383c54082900ced1b058cf697a76aa2d365ed6789aecaf7137ab2a11664b9a33c087451540f2062c31fe97cd6a15976e729" },
                { "hy-AM", "09e462ea8ae102d7c351abf8abd0295d59062c81c71cd1c290bf1362b00849f28b9c82da8d37ce8521eb800b0271705c78339625d9ae9b9323465c5c2f61e8cd" },
                { "ia", "5377854df978a918c58cbf4aaeb42c68f6365dd6fc9fd23aa5709e0dba620b6942a2bd510c668846fd15c3f320a3735b0fe14189ea304a2db49f574e14d0e58d" },
                { "id", "c8f018412ab4cc7cae219c1419b7a3084467b4650de9d6cb154becd9af3f432079f9cdcce9255d26390ec5756d0f71e781132454aff9b5f510b5456c79212222" },
                { "is", "1f0110da2039bac477cccae1a9ee771f994fa247beb183f1f1663772d309d7641f99b93250d4a712ebdc7e6b7072e9bd0fb2dbfdb6947ab7e43e47277615782d" },
                { "it", "f95033b6903f1c2ef7cf21fd829bad3cc5263aa815928e0f58c56b515d2fc1f1b8c32d259195f51ff5a3ed8e42ad699cf97383a0c40a9d4976bf0def089a2f3a" },
                { "ja", "47478d4a9c65798f7d3aa2a7d4fb04c8bfa6112e43158762f619ad26306d8d2ba2f314c05c927e880413c8bb51281a39db7895c8a0e78ccd05d0d7467fb9fdf6" },
                { "ka", "11907391a9f2c491e2aaa626568a8adf68faa99a4a3e8193b918893e506d9f03f4e2fdb71dc31536e1950237f5719b87d29614a840863eaafe35bb39af3c3c07" },
                { "kab", "9966342df7faaf82dd60c76833e7ab6d222e36b55210bf215be4f37d79735c5d62bfb9b8258be68315a8472622dc97c106ea9e12759b5bc1611033b4e1b9f0ff" },
                { "kk", "c61acf80d2ca68ce78d4de37485fe9fac9dad748ac1e3482e9681ab5174410c6d9d22a925339043e8ed91c63f7dd1147f30be43c8d3a5146df83136fb4bb542b" },
                { "km", "61e403d7dec16fa5fd1abd4342dbd2a442674a6b87eaf760ad3bfc6e92d70e05a3326b53b98df3c38c3baadcc283aada0acdece98495ee4c3939d81b6b7b2a88" },
                { "kn", "1c6cd6d5d1bd0014f4feafea5744cdd50cec3e2cda41179a06a6446ddc3928d74b27dfb142fb4d137b60b52a81d48ea012a9e7aaeeebcbfd8e46eea944f4203c" },
                { "ko", "55840deafc3250d442d29ab7815453a23414ef5c55959265bd46af2bf46bc41a42d0bb5e90c798f165b30a84638ac2d85b85a6bf9d5aa25dd05d65e000d5d0cc" },
                { "lij", "0bdb6d75b217bc9b8d9b30bed1b8c3a4bf74b1ae44ef3b5710a396b382fcfa17e5927d03d68fcc25bd28de96e06bb61e0eba188cea2d569cc381475eb23a93a8" },
                { "lt", "9b4b421f0333e661415c632bcd73b36fd288c385bb86fab40066856ab9b000a1e0d5eaa708a109c2d90d4852baf30b1d7e567e5720816b10fde204342e127643" },
                { "lv", "26c5658f9c8b1e72bf6a3311f16e97c602e81a24836ae19d4b054dce5f726547750f658606f127ba579f6be51e75504a32911f447f7aacb6191cb65730f1cd80" },
                { "mk", "ec254574e39b60f501d85ddafdcfea250fb3188cc8861c80218758011ca32a81c5ad0d5f0f61a2bf84077c74abf5233ad184d25fbcae175f2737b62539bab5ed" },
                { "mr", "841c60d7c0e2d614e10ddd73f855634a74c6d6666ab9a8f8dae75a1dcca0b2886be9e810cb426c825b69d43f4f57ea67abd7999c116cb98cf67c35e31ba96a9e" },
                { "ms", "c92d44769a34b05f317e5141ff3f0152ee1e85d2fae292f44f0edcb332e034116c5ae8cc242a0748818cba1b92449c65466b5b1262394b0a6bae29764372b9e4" },
                { "my", "f05e58d3173c0157d216a756bfbc38cddbf09c09427831727e4f4433d41f239185ba629a0e22358ff570d076bf8324dfd2833cb2b45e9c3a190be04f4d571384" },
                { "nb-NO", "1463f331f02d82a74e437c353d76de5bee387beba530a6ca26048f356ae7530b4e2601fd00cf8fbd06dc374f052f29f365aa01d622cf0d73b2aeed9d53787224" },
                { "ne-NP", "8ed54aeb32bd39ecf669e2680f8a0276d67c478ae1d11581e7c2673c9913ea7422acc0839b3e5d89f4711d777085c0b3fc962d7f2c3276017e08fcc02feaa323" },
                { "nl", "af4e4f3a2ddb8f8600cc84815fa0df5ae38464a0c7659f14eeaa10883ccb6bbfc38e7f3040b4927517d862a01685872501c66cb437b7ccf2de154f766772f5f7" },
                { "nn-NO", "39c258ecb227f7808935d4857c0a39ad7795c815a1836fa171343ad2368fd42173222c29736e58b7b7a58206a2b83153ef3a813837bc8ccb873c7c013fac98c2" },
                { "oc", "caf73a7bd489c7fa909df74c23c4376b164fe008b580294879841714e5eab11b660fa3f9f86fb72ea294c8cc5a25f81f2d8aa5f63ba0b363ceb3a2eafbe6298e" },
                { "pa-IN", "3a3680bc8d161887f5a1a4973f9967818a75686579b31d087c3f7455b2d9e41825fa78011b88b7b19aba5d00f8fc659c68419a66d5b2a3d1b8551de08e345989" },
                { "pl", "bf23176c80ffdd74cdea9174c7aaab11adbff0fa44c468f598ee817f4ab452e46bb5b6dae6aa63f20f364793fe15ba547ada75de1d14342091a2f29af043c8d3" },
                { "pt-BR", "7d6665ab96201865afa6b1954c0c69c754380803b06fb2e960c9a6b394fe1a607dca4637af4eedb8925012bb1d8e5fc9125be2292e5c04408492a25f91c5b56b" },
                { "pt-PT", "cdc9751a1339e757948ee3270ec3c59447ee3445b73b6dd0286c25451eb30c2da27d8ebcfc029e3831360be96f98be40b78e7a93987db346fdf92af0ea04d33f" },
                { "rm", "b3de448fdf2a416294085f6b439c595e0d6b735b46ab2e7578412948f70d450fa19cce889c086208bb27e6819eef38ccf147b095487b97d4bc9117fe9c651b7e" },
                { "ro", "44f7a22c8d59b669df0b6ec2e4df2d1d87c8872b3c22639a8d0348f2b977f589d19f40a86780b0f4d97ad31c1c6b36a24cfcdcca372ac43a7f2e4f9c0baea866" },
                { "ru", "9dfec626caa92fb81b2f8a04784f1e6160808e70b7f058b9a10202c4285193ac4ed4ad739a4c533401c28dba1c07eacf13e677edf924e4eb1ff2eff095f6ceda" },
                { "sat", "4976e4c73befd1708a36897752334c2b68431ec90b742dc5dc88d32ef0b8f4c700109fcb67eb459ee9eb6fc6e26b7cfa9f1f5c595a10bfc587e0394f64f89756" },
                { "sc", "54b51a69892b6861348a8b406727ef3c9803345683b1df5379bbf6ef8a227fae3fbc4fbd08ee02ba1a8188b957257080aba85f573e244317ca80e28c4bab04a3" },
                { "sco", "ac9fb5516452f93b49ed857af9d6f894a8ac4905bbf6d907eaa96731b001cc606e71e5b161b56bc60cda9d32542729b2a983b48dc9962f139e8fb1ee1aeb7a78" },
                { "si", "b9fe271ff9edfaa19c36e2f00de0fedc19cef5bf592fa7dc33e2658442a3594257aada1abe7a54a0ce632256f4968b9fd99608595bff2ea5a0a6e4d98bdf363b" },
                { "sk", "310c03baac2df74692aed822d1bc160bc628dc8073edee83f2a4d5baaec7750bec697ab633d5b9112efb6af3bf8f1e59b8ede01a97e4a60ef0c9377eace81c1d" },
                { "sl", "97f7eda858aa33c3995136cddb94920da47c18b8c8845454fb32fc4e1b147271d51d67dd9abb9203b3818b0363356bd88d02154f71ece1be0af4b013b5aeccf5" },
                { "son", "b5555409469770abbff7d8fb737b6067ddf5bbc6064416fa2f688f98ccc92511fd190915013352e02d498492096b8a065d159426ad9d57e191fa0365890be743" },
                { "sq", "139d1b3eb5484bb4301a27f5403137e933eeac98ca30dc4d1767f6e41e29f2ce21d186d3acfe367d04baa9febf661292c7f824f40324c8bcda8b2df367916fb4" },
                { "sr", "540a8e9e99460dcb83ddd28a430b7e60136ac0ac81647a27b190dc0f234ee9e36c870132100ffa18f961d05fed606cc79147c6909f282119ca8df06676a4027f" },
                { "sv-SE", "718886249a995efa001e6744381017c0c01ea90dc17a58f247e531031c1a06b9e1975bf6b0fc07aadc23189333a9477f1e2506a24c149f05d0f567f476560c5d" },
                { "szl", "121246f68f93738ba76aee3f108c16775544cdfcb430ca7cd4b9a67f998180c9753eea1d326f9e9a543ba39101e7af71e26f0f5f85746b87f7e6143af9267bbc" },
                { "ta", "5f436065cc4eb582e24e113240b1990fc68d276375220b30e3603527815a96298d91553863357eead90ea8ff7bd05450ed0d3fcc53279fec01a193e22cce4d97" },
                { "te", "4b58777321f80fc94ad9db93f004962c5dfcdbfd18c9d84f6dacc8526dcea432401f29b9ebb1752f3f8357d688e8200a7674977c6aa635e53bef07adb881c046" },
                { "tg", "8a7f994258e624dc60e42c1adcf7d7a276242ca7676f671c7ba0125d304df468a5eefb4ea730335a45d00b97dd91d2e85690a5eec99ed24f8a36c89a565df4ff" },
                { "th", "d45349dfac66a088ef8866b8fcd2d7307124d8af61532ed5226d5b5071aed31651b148841f3d40afdc69526264c98a6e3575e08f2e50d6db16abbeba4937e634" },
                { "tl", "cbfdfd9393fb2edb246befe23fe4598a5679e9f10411afcdaf3da7472812e5f576d58d846279246aca4b32c477924830d9a012be3071304a2328a57c33c65f7f" },
                { "tr", "d05c3cd3dffe46325eb7aaee3c0283a7a1288caae97ee5434af65a6414d0904cb660e5108ce226200d7b730f49d6d5d749b5915352ce2e832b9d77a73b6230c2" },
                { "trs", "2909105b311830dc58fbe25b912038005725e764423432f0f24a1cc8d8a2341cf4dcccdc9c151f1477c4ad4cc0cf5d3adcf669893636117989f1f04652e62be2" },
                { "uk", "e1a584d76460b4ce3049c730a49de98413091e092c86fd267440ff77186d03125dd1dc73363f28256734248eec00b14227f125af6d06ba4675e663e5cf90f6a2" },
                { "ur", "4aac05835f02a75a26e67f171e690c570153461ec27d5b084c9e122162ad665270c384999534a63eecae8c121de4a72cdfe5572c06eac023b39efef5b645a257" },
                { "uz", "53b2bce0bd58c681cadafc1021cdc90fab316c61084afdd6593885eb2e8c5884b5dd75b1826e570fd29c37f5ba5745e58431cf042bb78203c11c66b19974ea06" },
                { "vi", "4828a94fa00189889b24bb1e3f9278296bba784c2ca88488e61ee419c51f2fa804f96b12903e931df64ecee1009a3c62981ee64d3497db294cb6ac5846053831" },
                { "xh", "2f040fbc950c0a26b5f932c9778999db3ee1ca909bde62575ae98e3f33f2efb46b2a10c3478bd7c213920fdad6b0d87aabdaff8e8c1faf9d22d76529b67013fd" },
                { "zh-CN", "ac872d21d18e86fcb78560b973cfb4c126f07def5a47ff82d2d6b9ccc41a2622bd87526d40c30e1bd70e018ddec9d6de4b7f5ca690f9d7d7fe2b1490f6ab4cd6" },
                { "zh-TW", "75be52763873e20889595ef3158aa6f14c3d4f72d4dc9b842e48eef9a67616a8306c22ddaddf61f3122b8a1f89fb9cca9352a1e576e25f81825eef685f6fedb5" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "063ce53ec9e8d7dabdcb6b2a4ec3617e5fbe6866923068a50a36a4f5c5f982d6fd0c6ff7c2ede2357ddc42af53b4d65bc824680280046e5e6db89cfdd9ead22d" },
                { "af", "f8f653693c5224c8f610fe00a00cca7059ad5718e2c86eed5f8e1911c94a42457c48f0bbc8ba0e30427771aad9856d9e9a14deb18865ec25623bdf75a610c253" },
                { "an", "959ab8b3812bb736cc4c5c82666e6af1174af8727bdc531155f8f442046f891b3d7b5c4556a7bc912f139e53630024a809a02c50928958b26d786edc0dd52751" },
                { "ar", "71b9cfb55ac8784633ff8e08b8f085b5c01884f27589f4db8ab31090effbcb5885e1f7516d822a51cbee53a600d9974d6e482aee2ef408d07eab4b885304502e" },
                { "ast", "cbc4ed90b59a5d8c32d0aef6ffb8b9bfca5568d60ca1f3288c7eeae54ca90093ff4015b87252a3f28c07fd2cb6aeb1a7b926f52d69eb9b251dfda76bd9c849ce" },
                { "az", "7abca79b8ba059581bfe1ff6fb5df96a638476c1e7505e270fe1684c28e2970daa60d94c7acfde72f266cc33bf06ab83196e7c05a5cf80a15c8678f816ca7f84" },
                { "be", "0aab74d25ecbbcda13be7e6bf11c0f4ee720ca8badf4e6a650b924914b1a70e775ce92767754ac6582b97bfe82c79936865934a6d202969bd6536ad9400ed470" },
                { "bg", "b7a8da3e06afc68e1eead9a77a9effcc878aabcc0d4554ab4c14c70d79158ebb5bb265b4ea3b4d0d13ceeb70252e9008380538b944b8add6ea5ac111a8c293ec" },
                { "bn", "9bc77ecc05ad4eb659ae629d9041dcbb4f1d848f6259fc49b3280bdc9f39384a17983ea0aeffecd49f9ffc20dff2e2aa1b822e34ff1c3c5fad41479ceaa4fb4c" },
                { "br", "427df1d49f38079bf6857d36ae1475f0ce37a87180ed646d53de5fd70fdd9c0339dc11beff7cce3656d8466aea1b02ae1af5e9a9f03b158f1723a15f1af97b62" },
                { "bs", "829b9c47e4cd8f68001bdd4e0dd8cd40b3b1d5b986bfef3ee6fe6ee233029a6bcb99822edaef1574304ba6babbf3309696d74816b84aab77f508664c065302f9" },
                { "ca", "894fa786beab48d1b063ab879ed6a8f19d7d18e77dcb248e7a50ead70622e647da519c4b5187b68992d105f1fda91aa24f98a8572c14b5e737e3738f80565f83" },
                { "cak", "b5ba2a0ef7d8ee54c39cad4e88316c3d0a10cead715461e437940ac8c184450da8bdcd88981862dfd466a7f19bfa34d73551f9bcc2740e6aa64b7e170128d040" },
                { "cs", "0bdc3566ded391990a447c6981d55b925830119cfff0afbfdddb0cd0b90ed63a6053a05af3e0ec34d9fbc57219948f289fc6dbcfa6f41a71878cf66e9c99b2b6" },
                { "cy", "4818a2068028a7f6bab30325bd64065e1d88220d81d5aadbc75c42826b2bcb23ecb8ee90acd4f44f4ceace81f7d18628b4ddca4cd08c455c4492be60e6ffe9ee" },
                { "da", "7084cec0cb132845c35af409132165e07494bae68a43c7e5724b65054c80ca175a8bc8d0b904036241fa711a70e6e14d6fcd82107d892b613954956c0e7643f1" },
                { "de", "d69d5d0afd3eea6da47831e474e761d5db06fd81b5e79ab56b9f5cab4192f493a4d9be2d0f8c66308b8b427729eccd4df6daecc56563b51ba161f539ad7e519d" },
                { "dsb", "d1816cfdca53594c38a8231ef48b96a9d7b3acae35b140ccabca3eaf46009127840fa89409fd61e1b0308f7edf92f313cc20bae769e166fe6d34573290721e15" },
                { "el", "56462b4309475ab93fdc2ffb2335d9d9dd62eae8da1c9a8ddbd7b2718bfd9ea535295942f5f1bfa9bc9ecc2cda433aa3b0aaaddffbd5ff2030bdfd5d34919a35" },
                { "en-CA", "fc34c979f254b7c77743cfdfc7c11871f693fcca9393683b494890f7c8c382e18918ff1cdc98e3a85534027754ffd8f372208c7f7b791dcfa92c7ae549302d71" },
                { "en-GB", "f6947205b0a741dc7967368c81a58d88850b701bd593d6675bde8e6fa343efe458bd66854f4a0382d40ac399db32be388f78ea1fc7bdf0a7a6578fb3555a0634" },
                { "en-US", "4eabc29d97c9cbb7ccb13170d844d9056af0d55619fa852e7af7c3b77ff058b4f8b2aedd56189ba9f98fa3da7474d7dc55f7839e9ef4b87c5bb75ea5c71de1ff" },
                { "eo", "ec86c25030a52946b54dfdd6ed73c0a5de9e6defe1f239865b3f6b0dfae4dc6af7e6a33a79696a12b63b84b5e625cdcb3bc758e18150961a25ef882e50c8d210" },
                { "es-AR", "ea5bf409a4ec4891e33e0ac8b5aa4026abf4e678be6864a9f5ba2f1f1c312b4b216d9f51e06e22fcfc94a801975f97f30c02ab8225794a9aae6f830dbffe0e66" },
                { "es-CL", "0d6d08603cf455fec315cb75ac12c2a70b1d9ca034eeb747b971007c06eacb9babc279efdda5b185d040bdfec45b6ac07c39095bc4631ce2492fe79e4270f811" },
                { "es-ES", "3c90bd3b2f226d456d7b4a0498ac8aab45a9bbfe8f785381daa248653e215fb2bd6f9d940837ce2f4ad336bbf3333c0cee928c0dfdc75003e23633f13376a52f" },
                { "es-MX", "d28108db31e3cfd1c8f3e863107fdcb822255ec9c286dd70a4630371489919820d37c7d40d6188448395de615c42961c44d45528de77e8bdcc6c6d2e42f8544b" },
                { "et", "842523013815e289458a54b7aaa972be50fa8bfa06beeb3dfbd68608fd5dd0982d53d7366850d3101382a4be8da516fcd6f4488edfbbc92f7c75e42308513a19" },
                { "eu", "7183f662a89751a40d7a5ce471be6c32359cddd87021b9895ec2df782df901cf5da094f44c142865e97e638476ac9f8603a2148f8141a1eead3dc05db1b1e359" },
                { "fa", "de09d736bd4e05355826aae44eb3e7ebc3bbb32bacd1d97f51c04946fd9d726eecbc8fb66a1b5b5153759c37d63625f6be93c1f3754038eb4ad20ebaba039676" },
                { "ff", "459f796b9c782a4c277e39fb5971426d336a70df314f4dbec1a041bb0466e5103eb261efc5e3332f0578aaafe8ff437602e4394aaac1f3ed45366b726ce94e17" },
                { "fi", "fb0522d438233e2848da4049cd0ac4ab5f96f2ff99f0acdaf8086c4547e4e6c633d2675496b0980f5585f97d7e097aec20c7f706012bbd9fe78b31a60071412c" },
                { "fr", "8c38fc105cd68eccae3569e593ca2a3ce2e9c8e0228d6b62011d9aebd72de887be6fe58a672b73af5d551ef474774d31bb5cafa9320efce728d5d76608428195" },
                { "fur", "947073f75f86e6061f5e2a83b341e6047bd93a5231f4d1ea5d48feec6fd3c4b93bb45163860b201cd91874deb35b37713ba293be566f32cfa124c2d8c3a0f9e3" },
                { "fy-NL", "a46ce35e948ad99aa27d9d9216d06b5fb5ee94c2ea7931ffcbd3ddeb46bf022d87dc1c2756befa58d32a8378215dc47ceef8a6b1ab9fdbfde33a2a74efdd54a8" },
                { "ga-IE", "9c5aed6031451deb093616f3b49c4348b976085954d49a3c3e48c19345418f92027b9f8ea58221012a0ea3c4d57a121872b18df384c1a1d0ae386fb265dfe7eb" },
                { "gd", "c1bb3f3a042caf2a16ec5a6ec5b0de7a94ee2fc6a58b48f7516b293ac4b0e4bf81f505002a4cb6ecb1fcd96d4381c310ba6e42b1b71ce3d7ec2a61aba8480f0e" },
                { "gl", "60e7bab6f39e31c1f468ddb2371a8f2d17093b2565639754129115b7ab3a54a488084df480c0443dae9ea9fc243c063322b219d77fabf1c601492997131a2b03" },
                { "gn", "2ce44e9615f4501a5f32ea29e83701f5faac99281f1fc8c5ebfb86211fddd7e9e300dee1d71bd7dd531fff675fa8fb4121e3c4651a9ead4c22fe422cad77fd4b" },
                { "gu-IN", "27a34a0ba3c0e114dfc43e333e507ca70f8a668767ed8e3444f5978e90324cbb454ed54702c70f81a5d562300935e79b84b49a92651215c3c2aef228024a3276" },
                { "he", "3f3eebbe588e751ab19236dc6e46df56c3aef33763e57f411e34425338f87becd6b1bb91820a62a0aec765a48b46a34bf00dff3835aefe066acd19b1504da4f4" },
                { "hi-IN", "e085a999076b5a10cd30db76616f3f5a4a00827dc71556d55db85df0525effc9b73b80a4cd582aa52b1b7af35a5841e752bf661ac46c798e02787f36e90b94de" },
                { "hr", "904c393a2e3dd6ed00ae6d78099317d8c890d76cb401c7b622e4b048c6deb922b0ade4abdbd027e346375ccbc55e14727d059948bb81530d5d8cdfbe28d30615" },
                { "hsb", "d4360efefcda0a480921d53415afda552efa9c16461bfa07a16724fbddb4f218404748d5bbb73caf15bb14a3f6afebf42b01f5e9399bf17704fc094f343d89fc" },
                { "hu", "aff5dacd934e30b9138d5f750931d9eefd992989863c9841fe08b69059f31560f69d77792247a3d74ba4a72443e6679959e6cd6fb01fae103d9f4268a8d11098" },
                { "hy-AM", "c7a21a186f938d905868bb0d9bcce33babb1062aca02eeb971e790bad86c1b854b32f7af060aa3906262796ea3221e37c20081a5a493288308181ad0e9f7eb98" },
                { "ia", "efce3a250ea5293a9932b5dfc36a21a6f7b18b71c33877022514844d2619df64d58c246f1ce82ceff4a17b3d0365f63849007d08332a2ec50e61a1232771b334" },
                { "id", "a51fa209b106cb53a661640596a7d6ea7d66fd05cb42a8eff6b824bea83f572aed79eb1294a3785edb908892d13699f71426c54d28c9dc8c53bd552c69986790" },
                { "is", "d7f5325777942f57bdd43049965572ba005c6962cb2f93c968a5fefd45a4cfc1e0d6f49e034bb9290f19fbc00bca5deba805bbe81511620b010e2dcadae41d71" },
                { "it", "307204969b753df361e14dc64c7032960dbb286aa6b57eeb060e940aa3e5f7c4d3f16fec094eb475f72aec755266800233cb38a1b2bf257410a01fea25dc2a01" },
                { "ja", "9fa87cddd6ef7e8abfc47ff6b892cba1a64b99524e268ae10c0e1d3ad6c889cc90569689b0bcb7e809ca82912c1b1147a706a8eaadc539517965e1a54be47bc3" },
                { "ka", "fa4103256c9254bfca514ebcdc2189b6dd5cb6f90e7573c5beda645730a2e97c90b80a060b06d26a28618c35f2804f58b015c772841efcf6b6953491b37d3578" },
                { "kab", "41bd650e7a6723125cf7f7335718a3b1a83bcf95a1bf63bde081db3adafc4203ff5d6c9e1d482b2a7c387e46c87a6abacc11dca51465549e39360513fb949bfe" },
                { "kk", "a5590744cecc081ecb2a5347423f956d26cd7d47296d8f2621a28c145a6e299e1c16756c1a83f0e7e4cf8ec79b8a4ed9f141479ced444478acca26505f7cec9c" },
                { "km", "6b87469f847972ecb418f1287b14b22d8f40b423a2b93406a07febd65a77c2f34abdc66bfed99588d6633d1c4e41e3cc334e5143719a514877b239ec3c477c1c" },
                { "kn", "72de5063367eb4a4ac9a58394783e41ebda55cccd706e06ed757a7c0cd112c00b73eb1ce25759078495fb97d89760f4831c860ffacf5dd32876d8e4c8ff56f14" },
                { "ko", "776610ae519e528ceb56a77c5e54bb97cd5fc428bd058d1b3cff07ae20e5bd796c2952aa63bc3520ae7eb13031e7cfb871f212d09deee45d5240c51316fdac73" },
                { "lij", "22b5f9311d5299cc1f6afdce995d9d8c5aecdb5ca0cd87cb745bb329caf9bb86d13a11d5fe61279668c8db12088748f59bff46cf599cda2d7f08246a5d9292cb" },
                { "lt", "9f68c73749824d55d894024136c2149656e7d6c4f525fbafe2107c4684520ed8d440b928ce1a657fe468476160195b0bc94eda0bc912d6ce332f424483d18b7b" },
                { "lv", "013c7f32da5eef9995d66059c07aaa57f0453776c927dcb57a6e91fb59a44af62c6bebffd91ff83f1e57d4be9cfdde4fb3a28d848f475978872c6b6f40df3436" },
                { "mk", "ab0c45a8d1cd4ea8535743a651873d413928a640a7bafc73717889a8e3174dc76a0130c3bd5861269f27f0f902462206440732f0a94de25ac0f88dc708289964" },
                { "mr", "c21f600926fac60666a23d2932abeec32bee4593533f14ed9871d04210767a49770c768e4485715a6475f1255f5315539dfce5dc3abb9dd0b0c7e436b4a5d0a3" },
                { "ms", "2278db161948a9d016ddc77f0bb05a2744964babe7d2362165fd82294587381bfb4f8c1d7406dd20d209761ab26227e0e20ff883b285e58c197de23a921fefd1" },
                { "my", "dea3714b688d4550d4d6ccb6d4c5abcf774b7f550cd3039fa669a39a13b13dc9f18ec21e05fb289cf74cb40d78f58c495688899873cc766bdbbef8c849dc1903" },
                { "nb-NO", "7f1b2633bb00c724743ae7f1d2c7c2471fa9ef072f65ca04947d328483a8e8b079a05628cf576e5f39807efce708494c3fcea68b7b4889c1310adc8b61e71418" },
                { "ne-NP", "cf9844f0bbebc19781cf46bda4ea744c5021628dc2534c0e90987a713ad7bc989a0b0b24627c8d1e131ffff9a846f723bf759e2d35ec3794cf0dc8b54022846e" },
                { "nl", "b9fb1d034c735594cf5d86ed5f4c8b5b02a29049b049b45147e7aa1218131be9c6c610c8c6e1ecc99ddf5d1d60baa5a5bc6a3238df13b2310c47795a7301a37f" },
                { "nn-NO", "d4bcafe146f0567e0f5ec0f0d8b7b64dacd8e28ea7bde7ac8d3bfe8409d033f87f7d5c1742b5eeb089c9406523960738d27467dd4492938b034e734423c8d0c3" },
                { "oc", "2bd34dfcc64728d0bfbaa7db6e750830ad14f9b94b4e8feba3dcf8b919f5d3121df56d7745134b5113ec817d1fc80ab43dfe5e445a7b0d8e4fb433b698ff6930" },
                { "pa-IN", "172e9ab839eb5e83976887c67dc075cced586cf24c086db726542e6dddb9ffb2314a3e3ac64d61cfee8080cf0d3975760541b094c532559bd9ea111f636d24f1" },
                { "pl", "0bc8faa6566f87ec44affc741d9fed37e2c896b01a69bc558b241f5c22e3a3da5d9cedb6f51752e6344bb123eae49c9bc0664d89f16943b9d4ccbdc3911140fc" },
                { "pt-BR", "e4e37311e73a63d7037463a9358af5db82ec36ff716b077cecf33ed728d403eab9e62ad2edaa3f6813943d9c2534810b55b6e3165d899bf3c20dc017cba438e3" },
                { "pt-PT", "067d88492cf9d86de066149b74634f470449554577cc74906620797c55619b079a4867311a2758e0219d049bd16ace6a3e5c7213cfb8adffada81c242f2afa17" },
                { "rm", "fe25acb66710f8ceedc133ea5281452e313aab752b9c043265d712ebf714b57d18f7d10f14f92553367b1cc4e0f77c5ba51efe4cd26fec27320dff6ee910a075" },
                { "ro", "c3c541c83b82c5815364369a4744c02dcd3a3ed7174f9d8538e0aa5f4867c6f9c2ab7625a6c9d20a69eb3124e2daff346ac80d4a4e07ba65bab0fee325498d65" },
                { "ru", "ef93962dfed3c028adfb9226950323d7add1e9bcdf31d6099d8844310274f332f30eae2e101990c505b323c59bc05a019efa9a970ebf2dc5bb89d3dd162343a1" },
                { "sat", "4814539b2fde2436c5a8b622c06947b1674ed84979d12e8533796fc99c025579507394130221c96d250bb2b394a01d4b66c322a3944d40d6e8cc1b2e37c0d449" },
                { "sc", "59fd768042d372faded8910fb9962dd1262e5eb94efbce67bf7e35182c670b6613431c1324510e2d38ead1dddbef21756026c3a9ac8833fdaaec6b54f1cfb205" },
                { "sco", "142e2a922e79b25130bec3b94c9a35b86aac3179267abc885a414c1e1a6a37e690cb0dfeda179d900123210866dcafe56e5449a2b9e650e40bcf7e5fe015d3b3" },
                { "si", "8cace37d846d838c06a00d1f2f1d48846cf0e3993b133e380c92a51028cd6a665183ea8afe5a58c365c834f760e41c5b103ed272a243255226c0979109f8af5c" },
                { "sk", "99c652b15cb64f5691b9df9a3ef528c1384938c6ac9913c57aeeec042d703911a95842cab6aa7c752f493b435d6052fbdc0c99f2995b4299dd432632165f1599" },
                { "sl", "ec1c6ed4dd5982257ecf47077a1bc0ef5b9c4d5517d6d52f03851979c05b3db6b6250b0b527be01e7d2c335624f67cec2e2528d1f0c75298595f33113d075c01" },
                { "son", "d1701fb0697586ceae9608617f53623f979a96639e2d4a471aae6175e7d9a6254f0a38edb2e6cb0279afc6fe2f8fbf11c8dc6afcdc716401c08a13ba69a6c7bf" },
                { "sq", "5ad400b0b056e670409794c205da6a80f23bb1c126dd553d8776d70c4e262aed51421c0ace0870d5f2abea7438f6dca0f6f1a68e1585f63ab0790811375b328a" },
                { "sr", "57c8b3b66f343b34caf25a96f8e498b8797b9d8bf44f4779d99cd473cadd58fd771760d936d5ec4ccbc83a96ce5bd489c579b5aed1d3d9fa66588a1d5683c398" },
                { "sv-SE", "be138177557f387637c9ba38ed73d715a6e41d7cf6749645d58224b1806307a57ff26b9ff5f4e2dd4f545df6493580b13c3343dcf76d868670c6a9b7afbf4891" },
                { "szl", "a504de1f7f2cdb68f4781edd14ac1f2c1c715c8420d755429bd0b0e4272a0807dd0582fb3d3b3d61a12df6efdac080594c484f6c445aefc253ba634cc8a7d86d" },
                { "ta", "295caeff6ab6378bf38b27959a1c1f6f31d1206fcc3e810a5a24ceba8abea057cf3213b51df14f34f755684ab38224fdd946dcdfa4cf0a11f92d70e52913a0cf" },
                { "te", "7590fc71828975de569d85f5b906bdd1539fb37c0d528996d1608c5c87248223b47e5177ccf3cd1c76eba559a31d0416b54febd9489bfbc32f905bf9cc5ea16a" },
                { "tg", "62ea9e2c86b40bd8007df9fd5211e91481bbea3805e457a2e516e4d5aa589e372f34ad0edcede01b1b6b755f510f68b5b76b63482d639eccb927bb57c263350c" },
                { "th", "ee89737d9c6b75dc558436d2814d4cf58e8a16795fd5e89b520282f11225ac46e0085ec31026490d3b6bc83ee821e996ea2dc253aee98c7f92fd7228a5ca8de2" },
                { "tl", "34cac5b816e389ed45f060add94df92d5aac7bc419b0e7482fab8daa761596ce98bbfe3790f56d970ca635fac6a44db5cf831d7ccec50fe9faa260cc3d2e8732" },
                { "tr", "e54ba2879e468775b4f30af55afe83c520096c9707dc61a8b7a631850f5f87757256d93c1822bf1f518f973d11b0da1009a90c04b6cf85a901030eef8290c1cd" },
                { "trs", "7fe20eb078652c4d48f5199e80184a41dd93860c9b46498dee88c7aaf757cfca55738bf0a86cecc19933a8f74d765fd4a3d5f4d3ea109b07b696b4f264e07e18" },
                { "uk", "a03a18264883cd00b040731582a4bfa394c5e6bb2a801988088c3f5eb4258ad5a523dbb85223438b723b4be4160524dece9f1162dabd46247a01864dd9f0fa80" },
                { "ur", "7bd57c6f3754c6ac0ec8663ca0f136f3365c4730767846e5f89275324408d602ef8da05aa92220f614b9fe45df3f2b4cebb6ca6506772ae94d3602c742c9fff1" },
                { "uz", "a62a0325b47ea30c7e76b7311c959baa375e795e580300d1ee3720b364da8445239750013402d155d8dcce0d9b0f4d57b939d731cd888d2b9947bcc2e2585d49" },
                { "vi", "ffc4f6217e554101e0fe950bc0b985e1921fa53ce7c96991fd30c9bac83158e0dc2cabfa0730d955457d3c0ed131d1a796e834c95678ad3547f7a7fc0de9e2c8" },
                { "xh", "bae66aaf51a305be125053afc617f72e7a42529b863c7c69ae989af9d1b9ed3be929051a58c0941474aa1062a5240d077d6badee2e30a81d2115cd09c33b7e1d" },
                { "zh-CN", "10acf4d944c2c8f701964ef492817e7bdb835588123c488c281749c6008fdbfa4cf2a8f34d210cb76f498a7e27fffad4f16b1869abd0d91e8f9f19f957dc1c8d" },
                { "zh-TW", "a28335622ee17173149362c7e4edd96d731ed33f27cf5a0bc853b91efc883b327ca611a24c80b69ab5d7ad1a7de8435be47c50dfd9d652c9283bc98611773ec0" }
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
        public static string determineNewestVersion()
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
