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
        private const string currentVersion = "105.0b6";

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
            if (!validCodes.Contains<string>(languageCode))
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
            // https://ftp.mozilla.org/pub/devedition/releases/105.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "67c52ff5ac1c97569a024167831ff2ff5b0138f82c63df3b5c7bf97e0fdb99c2812d7026397bab756ca2b9fa37c2634f13c5fae303f64c57cdcbb7f796421da6" },
                { "af", "594a71a52b85cf28e224a828c04325126436e8894b1d087d7d598427ff2048e7c90b3b8b0d028c426293f7c4bb2a05d886d74d7563b367de1692c2f7a1d96a5e" },
                { "an", "c93477d8ed6307ff08fa86f30482db88ac84ea69c784a880033f7befa8851b58fdcbaeabded72147bb780e9d0853a47c8352f16b6086cb87e57eaf646e59614b" },
                { "ar", "7e8421a6b74bbfc594a5c91da039c39d6acb79505d60e84ea391c94e6393be8f91c25fa2563cd46d14acbfcecff88af36232b7327af92b76ec231d41d88bb864" },
                { "ast", "e9a900174b7ec81dd2e769c630a819a297d5e9cf34da5222682578a008b5e48961ca865c6686c73cb03217409a6b5c4eb11876a3736e7ef5c3e11c3a9668cdc6" },
                { "az", "0eb7fe41a5c09af95921fde562606e7735b80f5370cbbb248185bd52f7ff5fcae5009debaee559552d816c714de1267a565f5e160bbfb34c596f9269c43ed28c" },
                { "be", "de8b67fa96bc98013cdabdb3423767ca0896ada4a6aeb7c2cc8e7d8730f04d5e1fdc38a6cd2aeda885544f2110b85c11a1353af3c51e875d8fdc21a392f3c66c" },
                { "bg", "080f9715c942c7313d129ad7fc5bbb0a3d6cad1f434fdfe423a7684b3d2f33c1cc2c1678844f2317e4baa761b32f76284ed1b9754abdab65b73bd1a53acde111" },
                { "bn", "134c4f93a09d6d77c4000e47bd7a6cd96dfd9f2e9a90e3d556e89198f429fe7ecc923afd547060618d34819382e8de15d4508527ee56d4e5956927d28f22ba8b" },
                { "br", "1bf3a8e4b699ca97cf0fda936ee787a70ca011f65178157530b1e88b49c372be47b75f82e0103bb42a2cae460407f7f4e595bcc8ed92430854ea3cb54a769469" },
                { "bs", "9af97f032e030aefcd332cb62c2146382c72aaca4a31706e43b3ea368f7b61e6f34a36bd9e68a4e046e1682b917ae17cde369952d91dc9fa0a47c5aa30445db2" },
                { "ca", "1cb7d6083ba2d1b4c348865bd4049343d5dc3b1355ad2ec9f25036a45ee4018bbfe4917761992d0d40064c63f9908d5a5d44917f5102bf7234ac30764bdd0bf0" },
                { "cak", "57a15e4e288cf30fdbed5de200e63ad7815f1fd2e11e6d5540e9d2194f7c4876cba6b3e021ffe68cd41bfe57b0e612b0b2bf139968ee883e92a79f999716a1a1" },
                { "cs", "664319d30679c40c766c594a20ceb5c47b880dea8e9fe722282a836d7ab3b370785ee2e5640c74eb6e10ab3a582673a9db904b588e8eea15bf9897fbc8611657" },
                { "cy", "48062512c3dbf5859674b68c5d4d6fc82fd1bd93b006d34838a3d2f58c0dba22880922bb9315ebcbbd8c002fc2c767fc479c1548be649ebe8c004f0f75a09306" },
                { "da", "c604cd3eb14ebad9aae97820183bea40a59884aa0004d9748a620913ddd072341c566325f1ea8d39aecadf7d4855e023c7f55a3c69a7026c82ecfdd645d064af" },
                { "de", "73d4832c9d8dfaa7a86e1f7cd9ba68f0c9c5ae97d2fa2d0a492253d85186a0f9dbad5b7ee8a1fe9021990fa9be8d003f29e4941ff4cf99acae0fe2ff471be6ad" },
                { "dsb", "816b0b4a3e1d622020e564551d7d154c953a8657070a244f0f6e07ccc1fd301c08c92891cee0e3016867ed9d1bf90c4f1b3634385880dd9df19f3d807ab5e33d" },
                { "el", "8d2abb0d5cff7a00e454d8eb9276995f59220da3398ed885a5d1b77fc0cf190e366ef0efe5daa1ac620d7ce0d740f5d718787214fa0032b8345349a2c93888e4" },
                { "en-CA", "d2126efde0f908b61d5ea0271589caf766241d712d8e90e203fb52493080ab1cdc5f9fd0d29ac094fe3ae7319a375877634c0ec3b16a7fc82b6aded4519e4cf4" },
                { "en-GB", "0dd797af72b7cd00237bdb27786a763f372d4ad69f10bb42880dd55cbcde8fc892aa250f29920689f9b8084ad307e57185b9390e9f936aee1182d503387f914f" },
                { "en-US", "7d4c66b86e676b0ccf965d2c7eb6b310c0e46886af8acbebbf9aa09b3db68c1e6084862c524b2cd50032db72cf78a34e08a2715b1da09ac7445ebfff60b1eeb7" },
                { "eo", "7b881559974189b93b71e6fc8f40ecc296c9056af74db88ea4e008457a8ca122e074b0f91b4e9ac8d8113491f1d833c4e5351f7c1d51493a952360d78e82f68e" },
                { "es-AR", "f01ec1d6fe8aa2e98337e9d4e714a238f5addf1a3183f381a2486dc4f1e04465194b4b1f76b4fc2adcc6132ff005476e842f9ca7c0080d47e1f7924d383a87b4" },
                { "es-CL", "c58836e878ef36d2fb40d63efd19d36e4fc6704c9e7e94df3128f68a3220bc9db9e1462b1e75eca149aff43702e7de533887706aa78c4a83daa7ea32a9892e5f" },
                { "es-ES", "8ee219227629268b3daeb6313aafd1d6c49fd2632914aa67a2a2a63a3b036cd2de78dc0d666942e9f5e1b1556bc0d515403a69beada678bea6fc81a9778527ea" },
                { "es-MX", "41a9eef0f2535c556a143dcca4b2dd852262c7fa016ebd7b205a2c81f13f68aa0505090fa59f0b6afe16db8d112ac1427a0a680bbded22ac7407d916a079783f" },
                { "et", "bfe79dcf82b038cc043660546cbc3e59d2633ff45019b2d0eaac01e376f7c18629d0fe521bb25c7d6b6647ddb957491f190226c354b77e4d2b14a27da15f9096" },
                { "eu", "94abab95bb74c0f1340e833c394e9291be39f9171a6e3c7fc20b6ec15e1f299e58951583ee146962926a54fc8040166ff3d404ad9ae281045b520045dccd6382" },
                { "fa", "a79b42c77c9d416aeed0db56fbb2726403c4d5e446a25e1fab811b031985b3219a0f6dcfdce46bae990cdb64dc69300365100d28977e0aa6f3d6ee4929fc8869" },
                { "ff", "9f7773a1ca91ae7cd3db3352921413bcf8fd9c5b409a7670a703804eb0a552a48d864762bcf1d7d573f364f17db8ec85d4aaf38e9cbc90d0af141d6d475acce3" },
                { "fi", "23bc60c3529a8ac123ec434988e1599a3f7244bbf97ce5e3c0e4c6662456e449ff5cfd6ed85ce2d4e8afbaa9eb5f8b454d048860987fb2f1a3d64eb301bd2a87" },
                { "fr", "e1598729f735f1e969e6c2021d1bc5334a7c2aab0783425184e45e5746900ace70073768944ae398c30c5704feaf101ffc1c60c81cc78407334a0ae80488dd20" },
                { "fy-NL", "bfba64f3ec046dfce3258e2191ccfdf88b23991c0081be113501e6a7074b6606cce509fb057c73ca1256aed171c4745a8cf7a91bfd0db43951c39984b2d3127d" },
                { "ga-IE", "695cd811916eb37dd8c02f1413566d24abca96c92a1f1be36f8f0b096426920534668870857bd4f6442716754c4b9ea370d64abbb082e41d4ce3d4c45ebcfb53" },
                { "gd", "629834834dface6a4c1377152168185480b165260022904ecb980b6747c61fa528500f98e6d0d6593dbcdd493c6675e066d205ea7b392a00bfffab7565206a6f" },
                { "gl", "70e68f1728e543693cdfb08cc0d1c94af096df3764a6829554da68680e07663d777cf5a21ee3573903094c889b9642c18a97687341e2f37088831ea2910219b6" },
                { "gn", "5146ac16b8565f1e1b72a094874c5741b2850991be4ffdd35d2997c2253dd94300ee3fd37ca4e93dcbfc391771683efaf53e2ef22481a307118e399d16cf2186" },
                { "gu-IN", "48cff4b3dbee1e2d99605b01caf3aafe8d9e4b533abcf5afa9ae1c616a200614c1c1d241e8366c7b18e0f80e7e71be616a72bf3e1e00755e7379e2a9984e96c7" },
                { "he", "2298a1e884819449ff6ddf5c2246f48bd89c67b9a318161a0bec0641980578522e211a561a68fcc5f95c9fc7075b1aa094cc3b7a52b9066f387081b202ba630c" },
                { "hi-IN", "3c4a9201f8dd0ddd5d93f94aa94015f5cab87766e2f2b8a9ef88e2407683cfb0ba0c15daa4b1dc9099004dcb6334180de411b3ffa5f980cc2d5d2aa7164c74cd" },
                { "hr", "57edb80fa09677d33266c12a4594a6e34df6eb30749d028042d332b5b658b770577a502c8ba98fd2c267997878c2f4cd2e7fcf6078ade7b37fd3058a22f2319a" },
                { "hsb", "ce2af5048d25df592ad09858b97047c613f1333152fca4ccb34508f8333cd15485619e966173d21b28e462abd420c0cb931b89a9f1502a41ebea47c3cae44aff" },
                { "hu", "f3fa85798c93c993da31ca06865df85f221b8c2120e4e810f8eefe734a43048934579556456751195e879c38314c07e6b12348baa9b14c007429ed865c457fb9" },
                { "hy-AM", "967aba24ba2f9db7542dc2015e219ebc6fd846dd05d4e2ab12423efaee5ed0713b127da2a11f95305a85520db0958e3abf159d5191b91c5628fb13e2c4c24108" },
                { "ia", "3bc8e06beb8cea3c9c0829f965f01807af1b2ea5071f4acd44f4b27d973e73442a56b655e7ebfe03988da6ea80961584b595b0e8675c5150015fdbc9d1bd208b" },
                { "id", "45a7420649d7dfa818a3177abf18eb053920da7cfc8e6b6ff4fd02d8c2c72993173b703048720e2913b0b16f1ccf7a5653aa4fee5a8aa49823c0010934c89176" },
                { "is", "cdc3cecfb0c9986c8b5c3f223d0f19af2e83cf83893ef1fcf272e73f80faa77b3e275482f1f1bec9a544c077a5f2dc1273d07860b56e5f5e62e7bccd904e923c" },
                { "it", "fe588cad1e29d30c60af5083359fc94c8e6a8e30ad80911c96f879beb0a72ca79f77b7155aa469f947c6ddf8128f8775a300c90630cff91c695f46e13d61c062" },
                { "ja", "7a17c821c45668e438c349c2e6fca8b55c5936d58dbc165b4a38b2ed9b13c03db15c2f5133dad425f13d60bf783576d7937aa100d1c6dc73210b5ec533aed0f9" },
                { "ka", "b85cae1cfe9f271574111f192f2438bb39d403830a8616f22229ae9ce469e4f4550a93a3a516fbae12c2f5473ba2e0ec66b02334d128e4995f00800e1ad3c08e" },
                { "kab", "2a3d107a4e5f0b5e56d154b203684cc327151c1a9ab227d6a8145c0c81d75f2c1a927785ef32cce9531aef7db7d22e5dff81a6a1edca225f2f2879497505f7d5" },
                { "kk", "34b9d7e462da6b4a01398b84c67fe596274824bde7f4a6e95e32718098fa5e3cc742612d4c5b4ab87711b6599581d0155ad065800db50811c92b7661504630d1" },
                { "km", "72081016677bd8465bbf59a3ee45569e57dd3474e5bc6d18407dcb07aefd4b4b502091096ce81a6fc0916e2cb3703fc51d11d090d8e9a5f18808b153d2d145e8" },
                { "kn", "6b5bbd383455edad65d85198618caf451ec3525421d2267087c2e85cba3968567e77751d5430602b30ea61ffbf75054bd328f6b0113e1f0b7efcb3b1a348e37b" },
                { "ko", "f2c178e6b208059ace9e79ab69d20aeab86c07c4d7784caa639fb6e238b6feebc86df9bd5b66782b4c1451028116589064c080d637aa9995640076cdf05b9b1f" },
                { "lij", "ddd6b54644b7723589336c72b889b8945db597d6e3d5186ab2af70be01573f921aa48dde1c82862c8cc640688012542d31b2d136c99a3327cb3160d23678c867" },
                { "lt", "188a7e9db627aa2ed4d7cc38e18636d175cd6c35115a2134e0e3e0bce6c41c82e4dadfd33885176bdd27685f2303e97f93dcb6ae536a6b4e214f24e131bded44" },
                { "lv", "aa33d0deda084cb367244c1db63d2e022a6ddb26cd36db6f6f3d84395a119b798d6ee41ddc22a5fca8176ef43284b7dd144ee7450cc47d0cefa955f9ead70960" },
                { "mk", "10b8c2bbad6b9da00d37a40e3db669581605683a8dc2586f75b967caa47ed8ef4718fdbb1c627d06109e2dadb6fba3faa7dd43de190bbb8a6a364bed36050d5e" },
                { "mr", "f9949259455a559a4c4bbe782989b01b2bd876aa2e688bb2804fdf116fc74c5afa5bc61d477bd677354b181e503260f3550eb2691ffbb27d0cfb2029745a5b14" },
                { "ms", "5d68ea1502f94b0e3938540d0bbcede41e5b5b19721324694a62802458f42718699d4fa1e547a0ffe291dabd0daa66defe5a1bbc81f889d49a8f799c740991f5" },
                { "my", "5c31b27d6d37bdc86b25df0670018c99151dc2f29ce9ecc64962829d7d25e63ae4189f136a4cd7ac4aec7262fc0ba9eef6074eb68ecedb427d3da3a56b9dcd18" },
                { "nb-NO", "dba77736b3aae65831207f2e2a58fe7bcdfd3abd37ddcb49ef75c5c1a857b69aa3021fd6943bbf82c6932de7ee67bc13e2ae82798103825118dff9ad19f33bb0" },
                { "ne-NP", "4e4fb0ee4d574328962f935f1679adfd93ec53be0c8c3ace027f6c19be8f0ed8884a8cc69c76214b68de13ab87f2db63a0a4a15278a7a4ceff52733cc794d000" },
                { "nl", "ca7a16164b10825ab65ded889e22853f11d6dae8fb58a7a84ee5b2023ff1a4a8df6d2fc1873dd417524aa8e9da3ed42ecbc069fe9d3533b0c6b6f2478e8c5ebd" },
                { "nn-NO", "a879a8f35430683a9cde5251bc2fec40480297808e2fbf7d354620311c6bdcf7532beadfb526f8937c96950ee53203985a7fd2006d168550f61c4cfdebaab6f8" },
                { "oc", "d14030bbda239f861ea4dca532723f08f7cb682a1a4e958c72fa8bd55a0c04f4ecc23c6f3e1ac9cf6beb2e7a4c50194e5d3fa2765248b760085c981862f90b4e" },
                { "pa-IN", "75be73c6626df3314d8ed3fb38157ec86e7db26d7e156724c266ed914d2612d3ad38049f2f074e6fbae288784bb508c301e465ad376a29e7b4dfdf1e46c11fca" },
                { "pl", "f28d12148fc27a5d535a7039c102feb1a7e46ec5d0f5e470273663591ecc764796a3d794e0ea05650c5db52d3a238cdcce9cd278cab99c90719e177ff892771c" },
                { "pt-BR", "890236196a5aff5e5dbdf73784586e8c0c540c0802685867848d4faa4ede370a2435d6dd1af5abaae985559ffe02d38db6073f23947518d5305dea28067ace78" },
                { "pt-PT", "2df7c7277f4434761f643508d0b092589056105dc7e200954619e23cc9c044035556e90f945356f020dce06aca273561eb3917c42dcbcfdeefbf74f59e7ec587" },
                { "rm", "1e60a4cbef8fc3e6850369f9437a9d5748aed071448e8be9ea4c77564dff7f92eba121b734fb6fcba9bb150c58df9ab3f4a2e4ab9559da281f6f289635a41b78" },
                { "ro", "4eef6c34936841316e043db91275e45a5e17eaf8e8098951b7488a04bb0a8291aa38e3035b6d8e5905de74aa17e7582de39510ee727aa4a5b13615c0fe9d430e" },
                { "ru", "d020042fe7cac2a5cb8f23e70e16a2cf605bacc157f29a270d757da1d4f6a382cfe6e816db105c8f1575048e60959ed0899544c902115cbf8fae96f3589bf16b" },
                { "sco", "8a8bc0f0e183b4eb10775f50376e4f5491d4ece5688608ed5b97ea991fa65b91acc4e3eb33022c6202d6b58912c263b65d2bd2ced7c25ac1ee45df4e2174753f" },
                { "si", "6c8465c1bc2b691a61c57f823d5c6e0ef6e1fc400cf0f2ad6e8e6ccf23680730bce6f670cdd77f49706dae6afdb5654ad5e7e3db247bc193df01e0f0cb9d71a0" },
                { "sk", "de71f5622e37e01cf13be4e1d84b16596984b18b4302ebc88a90f9d7bbd52a2771e6972a2a149ed3d7fa99e5909a0e624567bbb2b72a745be57da8ba7f4a76db" },
                { "sl", "427d998d40f29a8573af385fa01358bdc6fed738208441a84114f2df679289aa771810ceff01c120f4e850a1fc220d0d213e01bf0e9e6a0798c0eabfc4017412" },
                { "son", "14d7b1bac330bf71178cf1d83219c9b38502eedc27f52669dee6b294bf57180a259ad9a89138ce6ab4fa2c5fcac8c6ac1f76cc45be3e59ae13d8de512e13cc53" },
                { "sq", "1baf33b13f3f28fecdc2c38304dcebe25fd60d442085f99c88915559cada1e257a02f809363421c07c0f3e803ea174c026ef3058b0b39a8668c350dcac0d42e5" },
                { "sr", "90083df5b9a8273c53b6e3c12e24c49a078e12f2e316ea50b4c3f9e0bf089d77c4554a0c60a87ff6757194959618e72456ae93bfd67fa7f73d7427f47f2b4afe" },
                { "sv-SE", "df0ef786db507be7cf036dee1b8c3fdbafa1a1cc186ec2421f3a9166ad19be65ba894201c6e81f7a736545a6d9be45204d8920114fadb9354ee4c568118cb3e7" },
                { "szl", "369838435e520e9376be441be26f736c9682af3d605305b42f9ebbd16d70b59df2b9298b50ca1cc6e0dab290e7652fb57499e8e10fe8533ff58da11732fa8cbf" },
                { "ta", "656d66a71daa36ab7ff18a227320ea144acc1cbef117aa6278be877990efecdf62d135b07674442315d6eebef72ed32918f68dcfbd7dbc3378ac216e5863fad5" },
                { "te", "a4f105cfbce95ccf6f5004b8cb24461bb597afa19c08490266b5f2d95032f66437f14997025b279ffdfc0204d72ac613de108db8bdbae9a9970f12d06a0df756" },
                { "th", "8e11a5b8cfd41d2172f870497e2f7ad975ff6ab4912903d4922f9d741ffd15f5c29fc6773e37c410e6460f63f61ae1b1172d7adc7d98dac03f3f07870f543d43" },
                { "tl", "4feb0bc1e6a2021299cf51e4c88224f86b62bf206535c74e56a0d259628acb4750770af889e29704df960f6e545957a1462afa93e6816dae4cd04d0f895416a0" },
                { "tr", "293f40af3ae4c30ca158f0ec8968d177599456f6fab435ce5d6d7c5a82e085c53f504345a3383040349bf0801bbc44ca00499c344bfaad0fb099f6b35c707cc5" },
                { "trs", "7f417b2efe39efbc798a94fe492bbd0f8cbb9226f3d3232bc123243db1b53888cd16557ebb0356d414af844ac599341a7d236dac60415e0a0794a2eb5a041bd6" },
                { "uk", "e4e495f509d239a70a67cdb80abe2925c829ad262072e743c1be4ed2f0d98f225122efbbdbe7ecd898fbb0acf656c7594248a09ce9b7da130a1b78f94618ac99" },
                { "ur", "61e2af21eb4ef216a8c6388b92306ed6d5fea1453f5bf025e494bf431d76306a4cccb58d4def81986e77243a816b13089ae1305ed17f2255e2973bb8566f1986" },
                { "uz", "667054f81686065f4e1a319b74f51b22914bfd60a4fc50876fccdfdf2e67b45c46003bb42911074b40d4e4d719c92208517308e0d33b45870e0402e7eefa05e1" },
                { "vi", "2b3a17b5fa56505f2dc3112fd0292ad4923165375a042b46e2eb7abb43fe40ba188ccea95f78bb3ac5a6c4b1a0cd95e9bb89ea7be30f3145ca1b12df3bbac380" },
                { "xh", "84f2bb99588ff94b299d6abe16ec810b0e7d2cd5a562c3864bd02433fda49e31cd5635eb431b2bf72da39861885277860b98ca011a61044ab2992146a33bd7ba" },
                { "zh-CN", "d9d5ff318fd9095b43402e2eda4eb59718d054ce23b719865c2c2a5b30f56b0a5d343e7c9a99619e157cd8ec7e77a2e7513b73010d5eec002e7583ccc2f2e521" },
                { "zh-TW", "5eb6aa2f88a52342ce4b6b834eec6b903e0c89e118cab1739b51e5703c9d38d75c7279d93bcf5d66fa762d2d5066649edc815778666c6bad7012b9f11e0e9a11" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/105.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "73a4946827f16d8c3d62b7cc38c17ce98d38bfbf6156ee3cb5652633cb65ee68650710b2590ad24ce619b8b2c1cc2f94ec3aff61aba0e0df1de8c9aa0ff610c2" },
                { "af", "395b0c0898ddce350a6d08eab84ccef4eab3002fd9946f3488860580fc0de5e7215ca03203cffb7ad4fff091498b1c6d83456bbb453aec5f846a761b571fe455" },
                { "an", "e82c0ef2bc22d76a4d59f28a77d1a3c3c7f4659cd2a116bd94e8c0eff1b2d933447ca1957f6828fc4ea91fd9af95aa770e3289b1bc652d6abb67f232ab6ad27f" },
                { "ar", "0a0e751c865e7b86a12ca18eeed6f6650614c06cb855e3ebb06685fde00023eca8f32b6491cc196233b3d7d5a08a1891bb62b07b96bb2ec1e45a72e1767a8118" },
                { "ast", "dd1c2f8ae5ea548d160294973ecffc1f3f658b85759bd3d4fe6ef85e7b4fcde766333c586e981e0186d955c9d0d10c205c4e3fba9cc8b0b97baa5b4ca81004b6" },
                { "az", "08e7064b39973a1f2350cb63a0fd02b76e538e1e64496da741837d71cb1aa6791f1e0a99732d2b787171594f25bec49309cccd1b318a50c7bf05050c3bc968c2" },
                { "be", "bf574b349c5dab1573473bcdfd3fb2c6edbd90747d1bf56242b1ed20d463bc0c38075bede76680e889b271158ebba37c83b4e461969047cb03da9607fb41e4bb" },
                { "bg", "84438485d171f65dc9f1363a70e60e340fbbfaaf1916c111ac0086705e913c5bda986c61752d9d6973e15bbbdfee21e201f7513d4fd4cb5aa17dbe49bc02ffdd" },
                { "bn", "577fbfefa1d4421190f275b69ccd6a5f6dae3830d6366ea97a498e3d94017587810f9f60ae382925b51fd37c4e4ea8d16c1b3e3e76103842962e1888178836c9" },
                { "br", "2a6fe5b6533686457ad513b3b5ab1d5cc7feee0f54ada9845bb06ceb60d504a26d23875a8c5045b828c189fceb6afb2e14dbb1dc538284f8679ceaad8a92cdd9" },
                { "bs", "d086b4984f6b41b23fb532387d91204d771739dc9dfe34bd61bfcff7e6f3e6adcbffd407fec5a428f2595df9b90bcdf47e9dc54d208df8d323868de55ef1ab33" },
                { "ca", "2b40a10795a4feff6ab4e0db401eb524c3f197d9e6c85a9a7a88b178f2ea56f0f6bbf245f6fb68dfed3f0c740d8d3ad96cb363dd2f79d0448229725adddb0d0a" },
                { "cak", "cc44cba665c02da0f0622cb75a6e244f69690db8ba2d549524d358406360b704fad2499238dac72145fd794925694df14874cb16ffef51b36f82bed945500482" },
                { "cs", "fc22b1aec07fbc9ef7a463fd06311ac5ce7122ce941e2bee7532b9a40e25ddd8674911d23c197fb1263a17258b329d9298dfff5a1690227199e07bc57bec44ed" },
                { "cy", "898c8fc71b5511564e454776ae3794c3f31b770acf34a3d96ee6e56ee96ed565b2708ae43a486cf8d8bd7b0a909032267a441122a1ddc936e4002ed3f8b06df7" },
                { "da", "45440ac46b66bde4c8c682f55304bbba5dd2ece2a00799be14a839684c3843c5fc73991327dd5fca4101ac43dd034a550599b6ca3c2de0cfe548f861ee10c509" },
                { "de", "edc5efa98e385e5fbc3282d55317bc1d82a4f4c8a7fdb7604919d73d9d21d899f4422dfc840d7c0ab71a57af78672b294ae4d367844e19c5b9e4ac5db6844fbc" },
                { "dsb", "19d772faaa57d703072a77b7f95a822be1e7f099d1c80484345d0f62bf2a6de6af1e0e5aae33f48979e37f4e47a0c8530b3db5e6ed5555571c16d29af4ba510a" },
                { "el", "3698bbb4b4943149865e42dc3c805d2d5a982da42e497f878cefbd349a92a767e319a9c4a59408ea55c3f794893dee95972a395d2b4447a4bdc1884231223802" },
                { "en-CA", "778310c63ec17b641491f51c4bfbd8ef9f8cca589b7046f12c8e43ed91c1e2a64f89200ee7325c1855be183be6e6ac7be4d0a99530b30adf10118b017c998591" },
                { "en-GB", "99484e96d2a807b8e526f2a6f9e53fec549b69d05b05da168fd39a3d38dbf5b31f0b9f8e2e2ac3f0ae8eb93a2863decbf7c6e362e03682bb1f8452b36a0519ab" },
                { "en-US", "a99b7a903afdce9665c3c8c875c7b98052890c585b8388d3918772cd51487a4107a670866348c64f58fcc844fe454c1c3953fa02a3b88bb71d4c2d48f39b2a38" },
                { "eo", "b2ba05fd5f99f8bac9b0800017758d0e245668f5789195b37f0171955c736492ba586674cc967315ef25ea6a9ce1bd0b8890a21bf0240bbb91f181cfe51d0ca5" },
                { "es-AR", "927ab7cd81c6cf9828dcead282f55b99acd4a7fea767ff0b0382dcf39a325bbfcb7807ff531341c8b589a8ba7aac1e16b1e5007895ee919f97b07c2f76bb4c69" },
                { "es-CL", "7638d464bae7f86964ece92fb6c882429906d94364bc9d53c20f7f3fc710bca0f884cdd001c9edfa6ac6a19385c3702fe5ead2776f2e78cf148865277db7f8e5" },
                { "es-ES", "0050dbe616d4c2cca5128712737083ebaf7a713f46cb51c3ac00b88063d9197118c73921b466fe5b437e5d3508bfa61884ee4d9804df8a93de18bd0dc42a788f" },
                { "es-MX", "dcb976ef78f429286b99da8bea73e6f56a32070dccfc45b4ed788dad84f415c835b4548062128ad7c970e869100859bff19faec5212f64d5830b3b3f2099234f" },
                { "et", "92182d226d4826fc23cd5f062b0c45c31b536fec430a89b48f85555e64cf2ae496baeb1bc9793cb0829413a6adec7c04305275cc647a93192eee1fcc5548a8ce" },
                { "eu", "063d6dac362606f7515ca0dfb186cf2e56cd502db9b1bdbc9227438dcc5c6e689b5e3cbb1ccbbb323cf768a325730f383fe5d0494c1102f02cc372bf046136e7" },
                { "fa", "81bed908888eea2fb3fb5a242a993f9047366b1bbeb1187a81e1b501327997ca9300eea74610113515c37c7e0b3f2ac053ca04ad28451fa0e4c14ca239ee7dd0" },
                { "ff", "23f70b554179559d71f93e2283b172da5fe25a9d0250c1e5f280cd9792d291d5db604967d9a52c34b7e9b4d063eed3250523f7e416c9fd91782978cbf666d429" },
                { "fi", "9034033905c8e01dcaf648c13f6013c218ff459a3579ad7ba413576383c40f0a79c1e15d58cfa4e967df5a73e40c7a64cd148152d521815fbdd69e522c294839" },
                { "fr", "83a23ca87d053982ba3441a1fd398c0241a3e315b628d60a46f55a041c08c671940e42e541be7abfd52a9e166950da83d5d309d690f2068b15f44868ed9dd4b6" },
                { "fy-NL", "6595408d652dc06f37879ed038d38e8196b95e08ac034d2a1e4c4309db3dbf970d5b48a0b44a5f3d0e42916d4f7564952b35721572259049551d14bf809254bd" },
                { "ga-IE", "87666f1bfc9e7d99da82bfff79d9653c9bc7c11d718e88dd69508f06c78689d2d37b2ee4891fc87af64bfac596a518db57cc4584c809cb9b7d8101dfa07b9a55" },
                { "gd", "5c02f4b75f46190352a8c37188efbf52d3558a6f5456a0625b8cc68f8ecb9d2c70abce75c8f3f881362a99d46a9f6f5338b21b2734d7e840b5d8eb361713e698" },
                { "gl", "4aaec1f6a080aca24f8ad2e0aed9ce49fede84ab1d2a68b1eece29125acce726a6320ad88720f518f700ac1e8f8386b0835072ec5c3c79685b19f9166f080baa" },
                { "gn", "03335d3ab756c994e9f4e85e90af14bea498f329c6b4effd2b2b3f85ebc177c881a86d27db7a143df88a42c5726c006e61b88637d50c2bd0db5d2abb3c80d98d" },
                { "gu-IN", "43afc61a472ac8d4672f637868b2d01606adc7c31db9c5b1d14e742b0de0301897fa8f5af2c1b57918ae0607b50e7654858168905e527ba33bbd9571baf86a20" },
                { "he", "37b38dcdfc0e7456194c9ea13e0d5e0377277c68368a5a748d63cd12550a322b35c6d4fa53a4a8ed598fafdd7f9c7110b7e630e6a61cb6516bfc38f4d0d3e22a" },
                { "hi-IN", "89add04cc4c9a9f579c34570ef0add8b2953e4878dc3cfa440843eb8207c932fa731742cca6f88e6f04d378e7f1669a093d448f9a31851381612543520a202ff" },
                { "hr", "f0401f7b36015067fc509bbf13167ce5edbd91d00bad892af7b2ae3871d8a248c586e5bef94d254723a77facdc6a687ab889314af91b1d3ee1b82936ff6d2187" },
                { "hsb", "0258db73a4d9879fcbc01f1ab162ee44bc9fcf21c4e4843dabaf2a8dd00663721bd8d3dbdf7b6f8b0d590b21948272a0856fc7a5abc98467ba9ea565147ad613" },
                { "hu", "aa8daa806bc161fb5144e2a38fbb6cd03401a77cf547e5f8295f0d257a9cbd925bcb0df7e76f398d45f9900d2f5d0ddabffe25fe3c14c4d961a6cf6dab7dc10e" },
                { "hy-AM", "847cd7f79934051974ac64728bc19ce10abb1196f0350d7ad6f65ff0f199024989300689b88e8d53875b456298c7fd7d853bef33f2fab590d9b52a347ab28a6b" },
                { "ia", "6ca5916a257601a28d91073058834b979660503193357cd539751c538e3406b55fe33288ac569c965d9ef7ba35e0c3b9f49e5d8af3f02fdf0a299286a0ae10ff" },
                { "id", "a35c7d0be58ef7fac9b6471bdb94952c98314ed29c759e93bf19e48ff335fa1988fb3527699ff28270c6209c539b5707ecd726e901e1bf3e5210346721431ef0" },
                { "is", "10de91f17df2664fc361cb125a0c474c58a30b25fbeb08cb578d7744209cbff38f6379d6943895f8865cbefb6fe53207d0ccb769b37ea5487ce329fdcf034fcd" },
                { "it", "167606164506093d07ba9bc6765880eb51d970b1869643348e192f5f593536b677d15d3fff296b996a3d620cffe48d5310b3ba15b9946bbcde6c6bb76bce4f3f" },
                { "ja", "63cc193d1bd6ee084e66c3b2796f05305f9a9126048b2423e5c0df47968fc8919be839a3cf350bede835f6eee3759f053d2c6eb351173715799e2f11a8466cca" },
                { "ka", "c02fdeaf860700535c54c9087d8b631b68f0233829d279b25d1f7212e6d6ba10c9c7be6c73a73df99f34a392566f69cd0a635715b47b8cdae6252cad8582e80f" },
                { "kab", "2b729c24f369c86944749dc6d81442ebd5fdc006cb5aa8d0f56ed41a38b2a4b0804ea18da31c000a303e9468c1b8ce822e7464e80ac549890bcdcb040222a77f" },
                { "kk", "87915f287f697506924637dd8a3191321ea79fb8ae1ebd7020ed1144a24c93fe8710a842563c439de538cc4e0d71d529d2ab3413b341dc4157227a7633909b27" },
                { "km", "af5095e0717f2c0ed0a2644f0278d03a22f25d8040d5b2a45c6ef2b97bf0e3ea9ebb0331a8bc7defbb8077754914c98567f4469a7048024b32f54b5fc00d465f" },
                { "kn", "32873227167dcf29c665a81c6eb22214ad2fadd5f696e9e6d7661d5ef17ebe8c9b32c5bf6001833f262ac8e816266f82ba427f955c91b775d529ac0489d3d0f8" },
                { "ko", "39792178d0079683952349acde37313f181d480a068288ad0ce1134ec209362e6efa573f69426344e9ed33b0640eac30901873d9cc92a4dcf595995209b15be4" },
                { "lij", "b026c7867c96a0e3d7bab1afa350b9672718052744b1aadf61e51b02b2374526a19134cca6ffd70e472cba25e689376b4499373d30276a99d0a17c9e34abe8d2" },
                { "lt", "11a6830e416e3cd6f4dabfd4e7f29fda1b70bbfe7a82d97a321f4b4bdfaca03a4896b8fffd40fa0174d17161efe111f9ba4a702c8b9401dd80094a6614562bc5" },
                { "lv", "ebd7e6c3a0a4fe8bcd7d12b757e7a0d41cd1680c42359d2e0b1e2dc2bf65918d14ad7463e767df135eea4808217d08ce5df9ac2ad8df710c4ccfeea2354a7218" },
                { "mk", "0afb83c7f8fbff0c4dbeaf57a8072d5eaefd39cf8291f181446a40db98fa0edaddf389e3e43a6cdf8abddbb73cba157e17c1236003fbd8b2f773d2d4347b5438" },
                { "mr", "a5221f9fcea5f8abf9bf7004c2c20262c477e15cb50756981a6255d13d6325a63ad95c1c7a34ea8f2b86415ac934a0c09a5b2f83a9851178749fa94ddd434c28" },
                { "ms", "32cb29801bdfe78f9958ee90b584a94231516724dcbf345ddf123cc2794b247ea918e21184f9ce77ba687b462e1e5706111b9c8b8a5923283106e2008ad237c9" },
                { "my", "73fe281684b1c7e2e9e3c01b7ddd5acb765129519d3883a60d1db8d456166f12fb1cfc1a024051abbe324c63d0dda00077a086584e857ca4fe944b2a9871302b" },
                { "nb-NO", "0561ec9adc45ac2b643bfee8d8bb456cf45c226e53d92b21fbd350e3c33f4caf39bce0422890bd9efe81e2ed170b35a082e4ccdd32a716cca159d8fe74bd6d95" },
                { "ne-NP", "c7a81e6753da089b637a434f55856d818f9238f38218d29fc9b7a9ffe0824b8736584e6470afcb6b10f208104012e04118c3857e72ad22d2400a5e9d4d9311f6" },
                { "nl", "6dce3aa61d5d5fd464a4f4bf5f955a09e889084a3b93f34260b193556607b70eca6439080983292f296d2421e81b3a0290ad0e3b1853f9a3e8a2689f684ce77d" },
                { "nn-NO", "97721f01b3ec94cd8b33479ffce52147793c5df97cce1bb401a53b7b928951ba39bac8679f52c25264122e903457de6bf8d1fcc3eff094900852667309709114" },
                { "oc", "8152d8420dcf0d8bcd9b15ad5c386855bb1cc9194d0362c2dbef21942f0e1fb3a7d9dc4ef4964a68ec04117fef1c184819b5d1c8f073ffb4aeb75fa48540e751" },
                { "pa-IN", "438f8c8886d43ff26d97349864a632e3c889609770f9364f613ddd8eab7322c0e17c8ec4346429a90d0d37490d5270aaf47ba593b0df7eadb8e3825cfdf792ee" },
                { "pl", "57f70209fe4867b72630abb58e6d60176c308096356fa19c90ad619852c7579e9e30fceb56eb8bef5b8cee6bd66d2479bfc2a34fcb80cfc479fb5d022621d6a1" },
                { "pt-BR", "1ed96a31754a8c528064da69f39941baa8c2027c9c0a1ab76c4c716975ed4faba672e4379a57aae303c220e86a9298388bb8970dbc4a2aef6b4ddc43469e67eb" },
                { "pt-PT", "f929b8601baaea3e7733dda2be632611ee443438cd9222d3f91e61da2270b7d725e2c343234d450def25b858f897cd707681dd0978402e03c0a717494bdcad63" },
                { "rm", "6ac16f9aeec942eef2b7ca5238f99932ec223ddfe89f99d1f449e6d476c404b14a9e39d26b4de79d28e5d87ad09e8d11afeb939ef7a8628236c200e9f3e465ef" },
                { "ro", "cdb701d6d3f95216d3cdf0b2cd990073bd98d383ed95f3ec54e5d70458e439fdd64f0be5bd3f63889c40fd30d2c0696d130cce55ae9fb05e8226ae78745e3716" },
                { "ru", "fccef811d763e61116cbf2ca8e67302c7a378f997fc47bf6ac1b738b2e35ddb479403edc7d8c426292ee505dfa5ee8a8bd3096f222e4d460eb45e0a9a39a990f" },
                { "sco", "718d038958337c4ad73e21302e7d0a453d035bbb4ad78bc4b3821c9544c6b24a84222be2d81ae8d8b52c66542fe6902aa41da0adcc8b1ff24315f2055bbc2244" },
                { "si", "ab4c3407888f4da69dfeaf164d018a6f1dc7bc9921d92674fda298e8dc93b8e1a3b62953b3b5fc152b4188af09ae7084cfb05d41addef13e0cafc80ee7b8c8fd" },
                { "sk", "88a74a9e96fd13e001e47d16a17b65c8fa62bc2c72370db49aef685fa563e5ecf6fd246b09c6b4659de963697e136c9f5764e456f1b2ee63253f7997e0490b75" },
                { "sl", "232302260976772e2e43b468db2c11e7dd4be1db3662fffab635640a765a5de2ddefe8bca48bdf0d395d7b06e7cb83507ad75100edbf0482d1a34f00c71efe38" },
                { "son", "b32d160bc9611685e4c393bfe20ef8553492769b944dab18a45e713e4e37f899fac944d0c64c593d8a5d0ef5d3c1129f68f3e201da8c3c7664934aa43e94a1c3" },
                { "sq", "5049f74795fd6c2f8b1143ec8a41c971a440d9ae09e8dbaa818a401a9010140cb3cdbbefc4db77be96c6d7e1f595617eda191c248bca37aa3ebedc726c85ddcb" },
                { "sr", "e4634ce797353bf057196bd1f2b5baf52ffe7e41bfe36ae87c1243b01c44cc4df47d137d263ced6b13739afe75d1e634568fe05253eb3bb84bed523531d7fa41" },
                { "sv-SE", "e94b56c9d4b2b21ce6bd0415812b16b69420effde51c651f57350c80fa96451e98f97ce215c4e90b55fec1e935f61b38b91104f1527eca61f6b531ecbb64b0db" },
                { "szl", "93cc00bfdf823a1341c4f20384720b44d13c4c15c3608cff353be2f4752f32900007e8b389d0ff39ad36134a6aa60f92756852a6bfc6a53ce5fcc3939d6f4f55" },
                { "ta", "41bc618f2e5e67ca6f12740ad522a91d5053b7eb1a442c16461810efd0237d7aa5e95c9b1b7bd610b782992079afbb0d871bcb67198b9bc6773d5576bb48e768" },
                { "te", "b5206d1a11c2249800a3f46971c6bc05e5e2d4fee232a86ebffc18d9825960700334261ec99cb08d68e46904e25a0cdf4954965ab78bf8469161a363222a7921" },
                { "th", "7ee9266bc4e79bb6393524af2b07c31633255e52be06422fab518540be58b7fb6141f8b234c76af00da18cb67e5a63a61e468c98469fadb2ff42f411239de4cb" },
                { "tl", "4ddf5cb54a95596dd6f6b885361743ade4e59c2d0068426b7db468b1da23b7ad7b322b9a88cbd65ef79bc6294d08371421b34fc051c5359cc3ffb8b223fc21db" },
                { "tr", "0d347493dbd7f898192d594a129a5f73212dfa6c03f5b5fa3d4cf2c18d797659959206758e9c3a2dabbc58fc35e209be69caf7f7aa9dd8d6f35293e96f244227" },
                { "trs", "25743e777a5b3295d8b714f16ea0bee32e2d06c268bf299071dd0516043f50741a5760447d2e40150872289d16bfc0e1560910c77b6f6088c0f36fbbf46521dc" },
                { "uk", "e39a81dfe54819cefaf050813b2139b78f89198ec8f09c0e247c7eab203e731f27fff2b3374c7a5659d6af8e24906125e31ff34e559d4d2ae5110c4868df9a0b" },
                { "ur", "1e41fac02e0450aeeb74d9ca45ab5dfb2635f023c275d1812a0dd7a257c294586d9bdd6be471f3976bacfcad0c65f94d2b30015a0af7584603f002972d2a724d" },
                { "uz", "68020c6468746baca8e2c592892ac9de9d631d2429a820591d5e33d9cd2d8a5a5127c7b50f16a613889b6d87dd8046bc3145f7ee20b6a3024ed9678f5ed04afc" },
                { "vi", "3d097ff124a71c8644fd85ffae0a63bcdabd466332433981ae25e87b6fb8b965027a193172a7af4745972fe74537223e43c0a700e8c113b5282f5305bf42c1e9" },
                { "xh", "f81a5ced81106372eb5d665ce181e5f6d727c5b4b537258c5ca7b813a0efb3966df89661011770ee67e17c318fa1c6ded52cc9ab3f95f963091e26c51aa50b5f" },
                { "zh-CN", "c3c80a67c688021fbf729d4cbe93302c47652c8ecac7270312c58419976903d34fff58b8425965a7ddc79dfba47265243c74db5af437162e86342fcf41562ba5" },
                { "zh-TW", "64dff5677a9be97c2cc158a6b969b25843a6d64b9e11acffd5a223be7c634c042a9f6c6468377a4cf04d51e09705fe7e1ac4ac497b708008fac87f5260e13c25" }
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
                sums.Add(matchChecksum.Value.Substring(0, 128));
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
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
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
